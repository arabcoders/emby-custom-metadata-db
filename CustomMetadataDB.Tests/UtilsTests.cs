using Xunit;
using System;
using System.IO;
using CustomMetadataDB.Helpers;
using System.Text.Json;
using System.Reflection;

namespace CustomMetadataDB.Tests
{
    public class UtilsTest
    {
        private static readonly DTO[] _data = LoadFixtureData();

        private static DTO[] LoadFixtureData()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "CustomMetadataDB.Tests.fixture.json";

            using Stream? stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                throw new Exception($"Could not find resource {resourceName}");
            }

            using StreamReader reader = new StreamReader(stream);
            return JsonSerializer.Deserialize<DTO[]>(json: reader.ReadToEnd(), options: Utils.JSON_OPTS) ?? Array.Empty<DTO>();
        }

        [Theory]
        [InlineData("Show title One", "jp-814d9672e1")]
        [InlineData("Show title 1", "jp-814d9672e1")]
        [InlineData("Show title 2", "jp-814d9672e2")]
        [InlineData("Show title Two", "jp-814d9672e2")]
        [InlineData("Show Title Wrong", "")]
        public void Test_get_series_id_from_json(string dir, string expected)
        {
            var dirLower = dir.ToLower();
            var result = "";

            for (int i = 0; i < _data.Length; i++)
            {
                var item = _data[i];
                foreach (var match in item.Matcher)
                {
                    if (match.ToLower() != dirLower)
                    {
                        continue;
                    }

                    result = item.Id;
                    break;
                }

                if (!string.IsNullOrEmpty(result))
                {
                    break;
                }

                // -- try title.
                if (item.Title.ToLower() == dirLower)
                {
                    result = item.Id;
                    break;
                }
            }

            Assert.Equal(expected, result);
        }
    }
}
