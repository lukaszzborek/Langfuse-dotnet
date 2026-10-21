using Shouldly;
using zborek.Langfuse.Models.Dataset;
using zborek.Langfuse.Models.ObservationV2;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Tests.Services;

public class QueryStringHelperTests
{
    #region DatasetItemListRequest QueryString Tests

    [Fact]
    public void Should_Build_QueryString_For_DatasetItemListRequest_With_Version()
    {
        var version = new DateTime(2024, 6, 15, 10, 0, 0, DateTimeKind.Utc);
        var request = new DatasetItemListRequest
        {
            DatasetName = "test-dataset",
            Page = 1,
            Limit = 10,
            Version = version
        };

        var queryString = QueryStringHelper.BuildQueryString(request);

        queryString.ShouldContain("datasetName=test-dataset");
        queryString.ShouldContain("page=1");
        queryString.ShouldContain("limit=10");
        queryString.ShouldContain("version=");
        queryString.ShouldContain("2024-06-15");
    }

    [Fact]
    public void Should_Build_QueryString_For_DatasetItemListRequest_Without_Version()
    {
        var request = new DatasetItemListRequest
        {
            DatasetName = "test-dataset",
            Page = 1
        };

        var queryString = QueryStringHelper.BuildQueryString(request);

        queryString.ShouldContain("datasetName=test-dataset");
        queryString.ShouldNotContain("version=");
    }

    #endregion

    #region ObservationsV2Request QueryString Tests

    [Fact]
    public void Should_Build_QueryString_For_ObservationsV2Request_With_ExpandMetadata()
    {
        var request = new ObservationsV2Request
        {
            Limit = 50,
            ExpandMetadata = "otel"
        };

        var queryString = QueryStringHelper.BuildQueryString(request);

        queryString.ShouldContain("limit=50");
        queryString.ShouldContain("expandMetadata=otel");
    }

    [Fact]
    public void Should_Build_QueryString_For_ObservationsV2Request_Without_ExpandMetadata()
    {
        var request = new ObservationsV2Request
        {
            Limit = 50
        };

        var queryString = QueryStringHelper.BuildQueryString(request);

        queryString.ShouldContain("limit=50");
        queryString.ShouldNotContain("expandMetadata");
    }

    #endregion
}
