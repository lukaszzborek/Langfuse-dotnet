using System.Globalization;
using System.Web;
using zborek.Langfuse.Models;
using zborek.Langfuse.Models.Requests;
using ObservationListRequest = zborek.Langfuse.Models.ObservationListRequest;
using SessionListRequest = zborek.Langfuse.Models.SessionListRequest;
using TraceListRequest = zborek.Langfuse.Models.TraceListRequest;

namespace zborek.Langfuse.Services;

/// <summary>
///     Helper class for building query strings from request objects
/// </summary>
internal static class QueryStringHelper
{
    /// <summary>
    ///     Builds a query string from an observation list request
    /// </summary>
    /// <param name="request">The observation list request</param>
    /// <returns>Query string</returns>
    public static string BuildQueryString(ObservationListRequest? request)
    {
        if (request == null)
        {
            return string.Empty;
        }

        var parameters = new List<string>();

        AddParameter(parameters, "page", request.Page);
        AddParameter(parameters, "limit", request.Limit);
        AddParameter(parameters, "name", request.Name);
        AddParameter(parameters, "userId", request.UserId);
        AddParameter(parameters, "type", request.Type);
        AddParameter(parameters, "traceId", request.TraceId);
        AddParameter(parameters, "level", request.Level?.ToString().ToUpperInvariant());
        AddParameter(parameters, "parentObservationId", request.ParentObservationId);
        AddParameter(parameters, "environment", request.Environment);
        AddParameter(parameters, "fromStartTime", request.FromStartTime?.ToString("O"));
        AddParameter(parameters, "toStartTime", request.ToStartTime?.ToString("O"));
        AddParameter(parameters, "version", request.Version);

        return parameters.Count > 0 ? "?" + string.Join("&", parameters) : string.Empty;
    }

    /// <summary>
    ///     Builds a query string from a trace list request
    /// </summary>
    /// <param name="request">The trace list request</param>
    /// <returns>Query string</returns>
    public static string BuildQueryString(TraceListRequest? request)
    {
        if (request == null)
        {
            return string.Empty;
        }

        var parameters = new List<string>();

        AddParameter(parameters, "page", request.Page);
        AddParameter(parameters, "limit", request.Limit);
        AddParameter(parameters, "userId", request.UserId);
        AddParameter(parameters, "name", request.Name);
        AddParameter(parameters, "fromTimestamp", request.FromTimestamp?.ToString("O"));
        AddParameter(parameters, "toTimestamp", request.ToTimestamp?.ToString("O"));
        AddParameter(parameters, "sessionId", request.SessionId);
        AddParameter(parameters, "environment", request.Environment);
        AddParameter(parameters, "version", request.Version);
        AddParameter(parameters, "release", request.Release);
        AddParameter(parameters, "orderBy", request.OrderBy);

        if (request.Tags is { Length: > 0 })
        {
            foreach (var tag in request.Tags)
            {
                AddParameter(parameters, "tags", tag);
            }
        }

        if (request.Fields is { Length: > 0 })
        {
            foreach (var tag in request.Fields)
            {
                AddParameter(parameters, "fields", tag);
            }
        }

        return parameters.Count > 0 ? "?" + string.Join("&", parameters) : string.Empty;
    }

    /// <summary>
    ///     Builds a query string from a session list request
    /// </summary>
    /// <param name="request">The session list request</param>
    /// <returns>Query string</returns>
    public static string BuildQueryString(SessionListRequest? request)
    {
        if (request == null)
        {
            return string.Empty;
        }

        var parameters = new List<string>();

        AddParameter(parameters, "page", request.Page);
        AddParameter(parameters, "limit", request.Limit);
        AddParameter(parameters, "fromTimestamp", request.FromTimestamp?.ToString("O"));
        AddParameter(parameters, "toTimestamp", request.ToTimestamp?.ToString("O"));
        AddParameter(parameters, "environment", request.Environment);
        AddParameter(parameters, "userId", request.UserId);

        return parameters.Count > 0 ? "?" + string.Join("&", parameters) : string.Empty;
    }

    /// <summary>
    ///     Builds a query string from a score list request
    /// </summary>
    /// <param name="request">The score list request</param>
    /// <returns>Query string</returns>
    public static string BuildQueryString(ScoreListRequest? request)
    {
        if (request == null)
        {
            return string.Empty;
        }

        var parameters = new List<string>();

        AddParameter(parameters, "page", request.Page);
        AddParameter(parameters, "limit", request.Limit);
        AddParameter(parameters, "userId", request.UserId);
        AddParameter(parameters, "name", request.Name);
        AddParameter(parameters, "fromTimestamp", request.FromTimestamp?.ToString("O"));
        AddParameter(parameters, "toTimestamp", request.ToTimestamp?.ToString("O"));
        AddParameter(parameters, "environment", request.Environment);
        AddParameter(parameters, "source", request.Source?.ToString().ToUpperInvariant());
        AddParameter(parameters, "operator", request.Operator);
        AddParameter(parameters, "value", request.Value);
        AddParameter(parameters, "scoreIds", request.ScoreIds);
        AddParameter(parameters, "configId", request.ConfigId);
        AddParameter(parameters, "queueId", request.QueueId);
        AddParameter(parameters, "dataType", request.DataType?.ToString().ToUpperInvariant());

        // Handle traceTags array parameter
        if (request.TraceTags != null && request.TraceTags.Length > 0)
        {
            foreach (var tag in request.TraceTags)
            {
                AddParameter(parameters, "traceTags", tag);
            }
        }

        return parameters.Count > 0 ? "?" + string.Join("&", parameters) : string.Empty;
    }

    /// <summary>
    ///     Builds a query string from a prompt list request
    /// </summary>
    /// <param name="request">The prompt list request</param>
    /// <returns>Query string</returns>
    public static string BuildQueryString(PromptListRequest? request)
    {
        if (request == null)
        {
            return string.Empty;
        }

        var parameters = new List<string>();

        AddParameter(parameters, "name", request.Name);
        AddParameter(parameters, "label", request.Label);
        AddParameter(parameters, "tag", request.Tag);
        AddParameter(parameters, "page", request.Page);
        AddParameter(parameters, "limit", request.Limit);
        AddParameter(parameters, "fromUpdatedAt", request.FromUpdatedAt?.ToString("O"));
        AddParameter(parameters, "toUpdatedAt", request.ToUpdatedAt?.ToString("O"));

        return parameters.Count > 0 ? "?" + string.Join("&", parameters) : string.Empty;
    }

    /// <summary>
    ///     Builds a query string from a dataset list request
    /// </summary>
    /// <param name="request">The dataset list request</param>
    /// <returns>Query string</returns>
    public static string BuildQueryString(DatasetListRequest? request)
    {
        if (request == null)
        {
            return string.Empty;
        }

        var parameters = new List<string>();

        AddParameter(parameters, "page", request.Page);
        AddParameter(parameters, "limit", request.Limit);

        return parameters.Count > 0 ? "?" + string.Join("&", parameters) : string.Empty;
    }

    /// <summary>
    ///     Builds a query string from a dataset run list request
    /// </summary>
    /// <param name="request">The dataset run list request</param>
    /// <returns>Query string</returns>
    public static string BuildQueryString(DatasetRunListRequest? request)
    {
        if (request == null)
        {
            return string.Empty;
        }

        var parameters = new List<string>();

        AddParameter(parameters, "page", request.Page);
        AddParameter(parameters, "limit", request.Limit);

        return parameters.Count > 0 ? "?" + string.Join("&", parameters) : string.Empty;
    }

    /// <summary>
    ///     Builds a query string from a model list request
    /// </summary>
    /// <param name="request">The model list request</param>
    /// <returns>Query string</returns>
    public static string BuildQueryString(ModelListRequest? request)
    {
        if (request == null)
        {
            return string.Empty;
        }

        var parameters = new List<string>();

        AddParameter(parameters, "page", request.Page);
        AddParameter(parameters, "limit", request.Limit);

        return parameters.Count > 0 ? "?" + string.Join("&", parameters) : string.Empty;
    }

    /// <summary>
    ///     Builds a query string from a comment list request
    /// </summary>
    /// <param name="request">The comment list request</param>
    /// <returns>Query string</returns>
    public static string BuildQueryString(CommentListRequest? request)
    {
        if (request == null)
        {
            return string.Empty;
        }

        var parameters = new List<string>();

        AddParameter(parameters, "page", request.Page);
        AddParameter(parameters, "limit", request.Limit);
        AddParameter(parameters, "objectType", request.ObjectType);
        AddParameter(parameters, "objectId", request.ObjectId);
        AddParameter(parameters, "authorUserId", request.AuthorUserId);

        return parameters.Count > 0 ? "?" + string.Join("&", parameters) : string.Empty;
    }

    /// <summary>
    ///     Builds a query string from a metrics request
    /// </summary>
    /// <param name="request">The metrics request</param>
    /// <returns>Query string</returns>
    public static string BuildQueryString(MetricsRequest? request)
    {
        if (request == null)
        {
            return string.Empty;
        }

        var parameters = new List<string>();

        AddParameter(parameters, "query", request.Query);

        return parameters.Count > 0 ? "?" + string.Join("&", parameters) : string.Empty;
    }

    /// <summary>
    ///     Builds a query string from a dataset item list request
    /// </summary>
    /// <param name="request">The dataset item list request</param>
    /// <returns>Query string</returns>
    public static string BuildQueryString(DatasetItemListRequest? request)
    {
        if (request == null)
        {
            return string.Empty;
        }

        var parameters = new List<string>();

        AddParameter(parameters, "datasetName", request.DatasetName);
        AddParameter(parameters, "sourceTraceId", request.SourceTraceId);
        AddParameter(parameters, "sourceObservationId", request.SourceObservationId);
        AddParameter(parameters, "page", request.Page);
        AddParameter(parameters, "limit", request.Limit);

        return parameters.Count > 0 ? "?" + string.Join("&", parameters) : string.Empty;
    }

    /// <summary>
    ///     Builds a query string from a dataset run item list request
    /// </summary>
    /// <param name="request">The dataset run item list request</param>
    /// <returns>Query string</returns>
    public static string BuildQueryString(DatasetRunItemListRequest? request)
    {
        if (request == null)
        {
            return string.Empty;
        }

        var parameters = new List<string>();

        AddParameter(parameters, "datasetId", request.DatasetId);
        AddParameter(parameters, "runName", request.RunName);
        AddParameter(parameters, "page", request.Page);
        AddParameter(parameters, "limit", request.Limit);

        return parameters.Count > 0 ? "?" + string.Join("&", parameters) : string.Empty;
    }

    /// <summary>
    ///     Builds a query string from a score configuration list request
    /// </summary>
    /// <param name="request">The score configuration list request</param>
    /// <returns>Query string</returns>
    public static string BuildQueryString(ScoreConfigListRequest? request)
    {
        if (request == null)
        {
            return string.Empty;
        }

        var parameters = new List<string>();

        AddParameter(parameters, "limit", request.Limit);
        AddParameter(parameters, "offset", request.Offset);

        return parameters.Count > 0 ? "?" + string.Join("&", parameters) : string.Empty;
    }

    /// <summary>
    ///     Builds a query string from an annotation queue list request
    /// </summary>
    /// <param name="request">The annotation queue list request</param>
    /// <returns>Query string</returns>
    public static string BuildQueryString(AnnotationQueueListRequest? request)
    {
        if (request == null)
        {
            return string.Empty;
        }

        var parameters = new List<string>();

        AddParameter(parameters, "page", request.Page);
        AddParameter(parameters, "limit", request.Limit);

        return parameters.Count > 0 ? "?" + string.Join("&", parameters) : string.Empty;
    }

    /// <summary>
    ///     Builds a query string from an annotation queue item list request
    /// </summary>
    /// <param name="request">The annotation queue item list request</param>
    /// <returns>Query string</returns>
    public static string BuildQueryString(AnnotationQueueItemListRequest? request)
    {
        if (request == null)
        {
            return string.Empty;
        }

        var parameters = new List<string>();

        AddParameter(parameters, "status", request.Status?.ToString().ToUpperInvariant());
        AddParameter(parameters, "page", request.Page);
        AddParameter(parameters, "limit", request.Limit);

        return parameters.Count > 0 ? "?" + string.Join("&", parameters) : string.Empty;
    }

    private static void AddParameter(List<string> parameters, string name, object? value)
    {
        if (value != null)
        {
            var valueString = value switch
            {
                int intValue => intValue.ToString(CultureInfo.InvariantCulture),
                DateTime dateTime => dateTime.ToString("O"),
                _ => value.ToString()
            };

            if (!string.IsNullOrEmpty(valueString))
            {
                parameters.Add($"{HttpUtility.UrlEncode(name)}={HttpUtility.UrlEncode(valueString)}");
            }
        }
    }
}