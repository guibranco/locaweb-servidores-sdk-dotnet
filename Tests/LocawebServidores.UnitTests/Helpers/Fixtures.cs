namespace LocawebServidores.UnitTests.Helpers
{
    internal static class Fixtures
    {
        public const string JsonApi = "application/vnd.api+json";

        public const string CloudServersList =
            @"{
  ""data"": [
    {
      ""id"": ""srv-001"",
      ""type"": ""servers"",
      ""attributes"": {
        ""name"": ""srv-001"",
        ""nickname"": ""web-01"",
        ""status"": ""active"",
        ""power_state"": ""on"",
        ""plan"": ""cloud-4gb"",
        ""os"": ""Ubuntu 22.04"",
        ""created_at"": ""2024-01-05T10:00:00-03:00""
      },
      ""meta"": { ""actions"": [""reboot"", ""shutdown""] },
      ""links"": { ""self"": ""https://api-servidores.locaweb.com.br/v1/cloud/servers/srv-001"" }
    },
    {
      ""id"": ""srv-002"",
      ""type"": ""servers"",
      ""attributes"": {
        ""name"": ""srv-002"",
        ""nickname"": ""db-01"",
        ""status"": ""active"",
        ""power_state"": ""off""
      }
    }
  ],
  ""meta"": { ""total"": 2 },
  ""links"": {
    ""self"": ""https://api-servidores.locaweb.com.br/v1/cloud/servers?page[number]=1"",
    ""next"": { ""href"": ""https://api-servidores.locaweb.com.br/v1/cloud/servers?page[number]=2"" }
  }
}";

        public const string CloudServerDetails =
            @"{
  ""data"": {
    ""id"": ""srv-001"",
    ""type"": ""servers"",
    ""attributes"": {
      ""name"": ""srv-001"",
      ""nickname"": ""web-01"",
      ""status"": ""active"",
      ""power_state"": ""on"",
      ""cpu"": 2,
      ""memory"": ""4096""
    },
    ""meta"": { ""actions"": [ { ""type"": ""reboot"" }, { ""name"": ""shutdown"" }, 42 ] }
  }
}";

        public const string CloudServerStatus =
            @"{
  ""data"": {
    ""id"": ""srv-001"",
    ""type"": ""servers"",
    ""attributes"": { ""status"": ""active"", ""power_state"": ""on"" },
    ""meta"": { ""actions"": [""reboot""] }
  }
}";

        public const string ActionPending =
            @"{ ""data"": { ""id"": ""act-1"", ""type"": ""actions"", ""attributes"": { ""type"": ""reboot"", ""status"": ""pending"" } } }";

        public const string ActionCompleted =
            @"{ ""data"": { ""id"": ""act-1"", ""type"": ""actions"", ""attributes"": { ""type"": ""reboot"", ""status"": ""completed"", ""updated_at"": ""2024-01-05T10:05:00Z"" } } }";

        public const string ValidationErrors =
            @"{
  ""errors"": [
    { ""status"": 422, ""title"": ""Invalid attribute"", ""detail"": ""Nickname can't be blank"", ""source"": { ""pointer"": ""/data/attributes/nickname"" } },
    { ""status"": ""422"", ""code"": ""too_long"", ""detail"": ""Nickname is too long"" }
  ]
}";

        public const string UnauthorizedError =
            @"{ ""errors"": [ { ""status"": ""401"", ""title"": ""Unauthorized"", ""detail"": ""Invalid token"" } ] }";

        public static string Resource(string type, string id, string attributesJson)
        {
            return "{ \"data\": { \"id\": \""
                + id
                + "\", \"type\": \""
                + type
                + "\", \"attributes\": "
                + attributesJson
                + " } }";
        }

        public static string Collection(string type, params string[] resourcesJson)
        {
            return "{ \"data\": [" + string.Join(",", resourcesJson) + "] }";
        }

        public static string ResourceItem(string type, string id, string attributesJson)
        {
            return "{ \"id\": \""
                + id
                + "\", \"type\": \""
                + type
                + "\", \"attributes\": "
                + attributesJson
                + " }";
        }
    }
}
