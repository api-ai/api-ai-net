﻿//
//  API.AI .NET SDK - client-side libraries for API.AI
//  =================================================
//
//  Copyright (C) 2015 by Speaktoit, Inc. (https://www.speaktoit.com)
//  https://www.api.ai
//
//  ***********************************************************************************************************************
//
//  Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with
//  the License. You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on
//  an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the
//  specific language governing permissions and limitations under the License.
//
//  ***********************************************************************************************************************

using System.Globalization;
using Newtonsoft.Json;
using ApiAiSDK.Model;
using Newtonsoft.Json.Linq;
using Xunit;

namespace ApiAiSDK.Tests
{
    public class ModelTests
    {
        private AIResponse GetTestResponse()
        {
            var testObject = new
            {
                id = "2d2d947b-6ccd-4615-8f16-59b8bfc0fa6b",
                timestamp = "2015-04-13T11:03:43.023Z",
                result = new
                {
                    source = "agent",
                    resolvedQuery = "test params 1.23",
                    speech = "",
                    action = "test_params",
                    parameters = new
                    {
                        number = "1.23",
                        integer = "17",
                        str = "string value",
                        complex_param = new {nested_key = "nested_value"}
                    },
                    metadata = new
                    {
                        intentId = "46a278fb-0ffc-4748-aa9a-5563d89199ee",
                        intentName = "test params"
                    }
                },
                status = new {code = 200, errorType = "success"}
            };

            var jsonText = JsonConvert.SerializeObject(testObject);
            return JsonConvert.DeserializeObject<AIResponse>(jsonText);
        }

        [Fact]
        public void TestResultGetString()
        {
            var response = GetTestResponse();

            Assert.Equal("1.23", response.Result.GetStringParameter("number"));
            Assert.Equal("default_value", response.Result.GetStringParameter("non_exist_parameter", "default_value"));
            Assert.Equal(string.Empty, response.Result.GetStringParameter("non_exist_parameter"));

            Assert.Equal("string value", response.Result.GetStringParameter("str"));
        }

        [Fact]
        public void TestResultGetInt()
        {
            var response = GetTestResponse();

            Assert.Equal(1, response.Result.GetIntParameter("number"));
            Assert.Equal(2, response.Result.GetIntParameter("non_exist_parameter", 2));
            Assert.Equal(0, response.Result.GetIntParameter("non_exist_parameter"));

            Assert.Equal(17, response.Result.GetIntParameter("integer"));

            Assert.Equal(5, response.Result.GetIntParameter("str"));
            Assert.Equal(0, response.Result.GetIntParameter("str"));
        }

        [Fact]
        public void TestResultGetFloat()
        {
            var response = GetTestResponse();

            Assert.Equal(1.23f, response.Result.GetFloatParameter("number"), 15);
            Assert.Equal(1.44f, response.Result.GetFloatParameter("non_exist_parameter", 1.44f), 15);
            Assert.Equal(0, response.Result.GetFloatParameter("non_exist_parameter"));

            Assert.Equal(17, response.Result.GetFloatParameter("integer"), 15);

            Assert.Equal(5f, response.Result.GetFloatParameter("str", 5f), 15);
            Assert.Equal(0, response.Result.GetFloatParameter("str"));
        }

        [Fact]
        public void TestResultGetComplex()
        {
            var response = GetTestResponse();

            var complexParam = response.Result.GetJsonParameter("complex_param");
            Assert.NotNull(complexParam);

            var nestedToken = complexParam["nested_key"] as JValue;
            Assert.NotNull(nestedToken);
            Assert.Equal(JTokenType.String, nestedToken.Type);
            Assert.Equal("nested_value", nestedToken.ToString(CultureInfo.InvariantCulture));
        }

        [Fact]
        public void TestParseContextParams()
        {
            var testObject = new
            {
                id = "2d2d947b-6ccd-4615-8f16-59b8bfc0fa6b",
                timestamp = "2015-04-13T11:03:43.023Z",
                result = new
                {
                    source = "agent",
                    resolvedQuery = "test params",
                    speech = "",
                    contexts = new object[]
                    {
                        new
                        {
                            name = "test_context",
                            parameters =
                            new
                            {
                                from_original = "Moscow",
                                from = new {city = "Moscow", metadata = new {some_data_key = "some data value"}}
                            },
                            lifespan = 1
                        }
                    },
                    status = new {code = 200, errorType = "success"}
                }
            };

            var response = JsonConvert.DeserializeObject<AIResponse>(JsonConvert.SerializeObject(testObject));

            var context = response.Result.Contexts[0];

            Assert.Equal("test_context", context.Name);
            Assert.NotNull(context.Parameters["from"] as JObject);
            Assert.NotNull(context.Parameters["from_original"] as string);
        }

        [Fact]
        public void TimeParameterTest()
        {
            var testResponse = new {
                id = "ac5d6831-3c27-46b8-b213-05ddc92c9757",
                timestamp ="2016-11-09T04:45:21.648Z",
                result = new {
                    source ="domains",
                    resolvedQuery ="set alarm to ten pm",
                    action ="clock.alarm_set",
                    parameters = new {
                        time = "22:00:00"
                    },
                    fulfillment = new {
                        speech = ""
                    },
                    score = 1
                },
                status = new {
                    code = 200,
                    errorType = "success"
                },
                sessionId = "8f357800-2297-46c7-b001-a21c22912602"
            };

            var testResponseString = JsonConvert.SerializeObject(testResponse);
            var response = JsonConvert.DeserializeObject<AIResponse>(testResponseString);

            Assert.Equal("domains", response.Result.Source);
            Assert.Equal("clock.alarm_set", response.Result.Action);

            var stringParameter = response.Result.GetStringParameter("time");
            Assert.NotNull(stringParameter);
        }

        [Fact]
        public void ComplexParameterTest()
        {
            var testResponse =
                new {
                    id = "32d5cbfb-745f-4ebc-b284-54cbfdf3605f",
                    timestamp = "2016-11-09T05:49:18.414Z",
                    result = new
                    {
                        source = "domains",
                        resolvedQuery = "Turn off TV at 7pm",
                        action = "smarthome.appliances_off",
                        parameters = new
                        {
                            action_condition = new
                            {
                                time = "19:00:00"
                            },
                            appliance_name = "TV"
                        },
                        score = 1
                    },
                    status = new
                    {
                        code = 200,
                        errorType = "success"
                    },
                    sessionId = "8f357800-2297-46c7-b001-a21c22912602"
                };

            var testResponseString = JsonConvert.SerializeObject(testResponse);
            var response = JsonConvert.DeserializeObject<AIResponse>(testResponseString);

            Assert.Equal("domains", response.Result.Source);
            Assert.Equal("smarthome.appliances_off", response.Result.Action);

            var actionCondition = response.Result.GetJsonParameter("action_condition");

            var timeToken = actionCondition.SelectToken("time");
            Assert.NotNull(timeToken);
        }

        [Fact]
        public void MessagesTest()
        {
            var testResponse = new {
                id = "ac5d6831-3c27-46b8-b213-05ddc92c9757",
                timestamp ="2016-11-09T04:45:21.648Z",
                result = new {
                    source ="agent",
                    resolvedQuery ="hello",
                    action ="hello_action",
                    actionIncomplete = false,
                    fulfillment = new {
                        speech = "",
                        messages = new object[] {
                            new
                            {
                                type = 0,
                                speech = "Some speech"
                            },
                            new
                            {
                                type = 2,
                                title = "Choose an item",
                                replies = new[] {"Good", "Nice"}
                            }
                        }
                    },
                    score = 1
                },
                status = new {
                    code = 200,
                    errorType = "success"
                },
                sessionId = "8f357800-2297-46c7-b001-a21c22912602"
            };

            var testResponseString = JsonConvert.SerializeObject(testResponse);
            var response = JsonConvert.DeserializeObject<AIResponse>(testResponseString);

            Assert.Equal("agent", response.Result.Source);
            Assert.Equal("hello_action", response.Result.Action);

            var fulfillment = response.Result.Fulfillment;
            Assert.NotNull(fulfillment);
            Assert.Equal(2, fulfillment.Messages.Count);

            var firstMessage = (JObject) fulfillment.Messages[0];
            var secondMessage = (JObject) fulfillment.Messages[1];

            Assert.Equal(0, firstMessage["type"].Value<int>());
            Assert.Equal("Some speech", firstMessage["speech"].Value<string>());

            Assert.Equal(2, secondMessage["type"].Value<int>());
            Assert.Equal("Choose an item", secondMessage["title"].Value<string>());

        }

    }
}

