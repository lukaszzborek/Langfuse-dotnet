#!/usr/bin/env python3
"""
Split a large OpenAPI YAML file into smaller category-based files.

Usage:
    python split_openapi.py [input_file] [output_dir]

Defaults:
    input_file: langfuse-openapi.yml
    output_dir: openapi-split
"""

import sys
import re
from pathlib import Path
from collections import defaultdict
from typing import Any

try:
    import yaml
except ImportError:
    print("Error: PyYAML is required. Install with: pip install pyyaml")
    sys.exit(1)


def load_openapi(path: Path) -> dict:
    """Load the OpenAPI YAML file."""
    with open(path, 'r', encoding='utf-8') as f:
        return yaml.safe_load(f)


def get_paths_by_tag(spec: dict) -> dict[str, dict]:
    """Group paths by their primary tag."""
    paths_by_tag = defaultdict(dict)

    for path, methods in spec.get('paths', {}).items():
        for method, operation in methods.items():
            if isinstance(operation, dict) and 'tags' in operation:
                # Use the first tag as the primary category
                primary_tag = operation['tags'][0]

                if path not in paths_by_tag[primary_tag]:
                    paths_by_tag[primary_tag][path] = {}
                paths_by_tag[primary_tag][path][method] = operation

    return dict(paths_by_tag)


def find_refs(obj: Any, refs: set = None) -> set[str]:
    """Recursively find all $ref references to schemas."""
    if refs is None:
        refs = set()

    if isinstance(obj, dict):
        if '$ref' in obj:
            ref = obj['$ref']
            # Extract schema name from "#/components/schemas/SchemaName"
            match = re.match(r'#/components/schemas/(\w+)', ref)
            if match:
                refs.add(match.group(1))
        for value in obj.values():
            find_refs(value, refs)
    elif isinstance(obj, list):
        for item in obj:
            find_refs(item, refs)

    return refs


def resolve_schema_dependencies(all_schemas: dict, needed: set[str]) -> set[str]:
    """Recursively resolve all schema dependencies."""
    resolved = set()
    to_process = set(needed)

    while to_process:
        schema_name = to_process.pop()
        if schema_name in resolved:
            continue
        if schema_name not in all_schemas:
            continue

        resolved.add(schema_name)

        # Find refs in this schema
        schema_refs = find_refs(all_schemas[schema_name])
        for ref in schema_refs:
            if ref not in resolved:
                to_process.add(ref)

    return resolved


def create_split_spec(
    base_info: dict,
    tag: str,
    paths: dict,
    schemas: dict
) -> dict:
    """Create a valid OpenAPI spec with subset of paths and schemas."""
    spec = {
        'openapi': base_info.get('openapi', '3.0.1'),
        'info': {
            'title': f"Langfuse API - {tag}",
            'version': base_info.get('info', {}).get('version', ''),
            'description': f"API endpoints for {tag} category"
        },
        'paths': paths,
    }

    if schemas:
        spec['components'] = {
            'schemas': schemas,
            'securitySchemes': {
                'BasicAuth': {
                    'type': 'http',
                    'scheme': 'basic'
                }
            }
        }

    return spec


def tag_to_filename(tag: str) -> str:
    """Convert a tag name to a filename-friendly format."""
    # Convert CamelCase to kebab-case
    name = re.sub(r'([a-z])([A-Z])', r'\1-\2', tag)
    return name.lower() + '.yml'


def main():
    # Parse arguments
    input_file = Path(sys.argv[1]) if len(sys.argv) > 1 else Path('langfuse-openapi.yml')
    output_dir = Path(sys.argv[2]) if len(sys.argv) > 2 else Path('openapi-split')

    if not input_file.exists():
        print(f"Error: Input file '{input_file}' not found")
        sys.exit(1)

    print(f"Loading {input_file}...")
    spec = load_openapi(input_file)

    # Get all schemas
    all_schemas = spec.get('components', {}).get('schemas', {})
    print(f"Found {len(all_schemas)} schemas")

    # Group paths by tag
    paths_by_tag = get_paths_by_tag(spec)
    print(f"Found {len(paths_by_tag)} categories:")
    for tag, paths in sorted(paths_by_tag.items(), key=lambda x: -len(x[1])):
        print(f"  - {tag}: {len(paths)} paths")

    # Create output directory
    output_dir.mkdir(exist_ok=True)

    # Generate split files
    print(f"\nWriting to {output_dir}/...")

    for tag, paths in paths_by_tag.items():
        # Find all schema refs in these paths
        needed_schemas = find_refs(paths)

        # Resolve dependencies
        all_needed = resolve_schema_dependencies(all_schemas, needed_schemas)

        # Get the actual schema definitions
        schemas = {name: all_schemas[name] for name in sorted(all_needed) if name in all_schemas}

        # Create the split spec
        split_spec = create_split_spec(spec, tag, paths, schemas)

        # Write to file
        filename = tag_to_filename(tag)
        output_path = output_dir / filename

        with open(output_path, 'w', encoding='utf-8') as f:
            yaml.dump(split_spec, f, default_flow_style=False, allow_unicode=True, sort_keys=False)

        print(f"  {filename}: {len(paths)} paths, {len(schemas)} schemas")

    print(f"\nDone! Generated {len(paths_by_tag)} files in {output_dir}/")


if __name__ == '__main__':
    main()
