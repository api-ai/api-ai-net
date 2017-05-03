//
// API.AI .NET SDK tests - client-side tests for API.AI
// =================================================
//
// Copyright (C) 2015 by Speaktoit, Inc. (https://www.speaktoit.com)
// https://www.api.ai
//
// ***********************************************************************************************************************
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with
// the License. You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on
// an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the
// specific language governing permissions and limitations under the License.
//
// ***********************************************************************************************************************

using System;
using System.IO;
using NUnit.Framework;
using ApiAiSDK.Model;
using System.Reflection;
using System.Threading.Tasks;

namespace ApiAiSDK.Tests
{
	[TestFixture]
	public class ApiAiTest
	{
		private const string ACCESS_TOKEN = "3485a96fb27744db83e78b8c4bc9e7b7";

		[Test]
		public async Task TextRequestTest()
		{
			var config = new AIConfiguration(ACCESS_TOKEN, SupportedLanguage.English);

			var apiAi = new ApiAi(config);

			var response = await apiAi.TextRequestAsync("hello");

			Assert.IsNotNull(response);
			Assert.AreEqual("greeting", response.Result.Action);
            Assert.AreEqual("Hi! How are you?", response.Result.Fulfillment.Speech);
		}

		[Test]
		public async Task TextAIRequestTest()
		{
			var config = new AIConfiguration(ACCESS_TOKEN, SupportedLanguage.English);
			
			var apiAi = new ApiAi(config);

			var request = new AIRequest("hello");
			var response = await apiAi.TextRequestAsync(request);
			
			Assert.IsNotNull(response);
			Assert.AreEqual("greeting", response.Result.Action);
            Assert.AreEqual("Hi! How are you?", response.Result.Fulfillment.Speech);
		}

		[Test]
		public async Task VoiceRequestTest()
		{
			var config = new AIConfiguration(ACCESS_TOKEN, SupportedLanguage.English);

			var apiAi = new ApiAi(config);
			var stream = ReadFileFromResource("ApiAiSDK.Tests.TestData.what_is_your_name.raw");

			var response = await apiAi.VoiceRequestAsync(stream);

			Assert.IsNotNull(response);
			Assert.AreEqual("what is your name", response.Result.ResolvedQuery);
		}

		private Stream ReadFileFromResource(string resourceId)
		{
			Assembly a = Assembly.GetEntryAssembly();
			Stream stream = a.GetManifestResourceStream(resourceId);
			return stream;
		}

	}
}

