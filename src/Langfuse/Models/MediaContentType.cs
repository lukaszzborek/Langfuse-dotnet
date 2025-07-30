namespace zborek.Langfuse.Models;

/// <summary>
///     Supported MIME types for media records matching OpenAPI MediaContentType schema
/// </summary>
public enum MediaContentType
{
    /// <summary>
    ///     PNG image format
    /// </summary>
    ImagePng,

    /// <summary>
    ///     JPEG image format
    /// </summary>
    ImageJpeg,

    /// <summary>
    ///     JPG image format
    /// </summary>
    ImageJpg,

    /// <summary>
    ///     WebP image format
    /// </summary>
    ImageWebp,

    /// <summary>
    ///     GIF image format
    /// </summary>
    ImageGif,

    /// <summary>
    ///     SVG image format
    /// </summary>
    ImageSvgXml,

    /// <summary>
    ///     TIFF image format
    /// </summary>
    ImageTiff,

    /// <summary>
    ///     BMP image format
    /// </summary>
    ImageBmp,

    /// <summary>
    ///     MPEG audio format
    /// </summary>
    AudioMpeg,

    /// <summary>
    ///     MP3 audio format
    /// </summary>
    AudioMp3,

    /// <summary>
    ///     WAV audio format
    /// </summary>
    AudioWav,

    /// <summary>
    ///     OGG audio format
    /// </summary>
    AudioOgg,

    /// <summary>
    ///     OGA audio format
    /// </summary>
    AudioOga,

    /// <summary>
    ///     AAC audio format
    /// </summary>
    AudioAac,

    /// <summary>
    ///     MP4 audio format
    /// </summary>
    AudioMp4,

    /// <summary>
    ///     FLAC audio format
    /// </summary>
    AudioFlac,

    /// <summary>
    ///     MP4 video format
    /// </summary>
    VideoMp4,

    /// <summary>
    ///     WebM video format
    /// </summary>
    VideoWebm,

    /// <summary>
    ///     Plain text format
    /// </summary>
    TextPlain,

    /// <summary>
    ///     HTML text format
    /// </summary>
    TextHtml,

    /// <summary>
    ///     CSS text format
    /// </summary>
    TextCss,

    /// <summary>
    ///     CSV text format
    /// </summary>
    TextCsv,

    /// <summary>
    ///     PDF document format
    /// </summary>
    ApplicationPdf,

    /// <summary>
    ///     Microsoft Word document format
    /// </summary>
    ApplicationMsword,

    /// <summary>
    ///     Microsoft Excel document format
    /// </summary>
    ApplicationVndMsExcel,

    /// <summary>
    ///     ZIP archive format
    /// </summary>
    ApplicationZip,

    /// <summary>
    ///     JSON data format
    /// </summary>
    ApplicationJson,

    /// <summary>
    ///     XML data format
    /// </summary>
    ApplicationXml,

    /// <summary>
    ///     Binary data format
    /// </summary>
    ApplicationOctetStream
}