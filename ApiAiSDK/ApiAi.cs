﻿//
// API.AI .NET SDK - client-side libraries for API.AI
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
using ApiAiSDK.Model;
using System.Threading.Tasks;

namespace ApiAiSDK
{
    public class ApiAi : ApiAiBase
	{
		private readonly AIConfiguration config;
		private readonly AIDataService dataService;

		public ApiAi(AIConfiguration config)
		{
			this.config = config;

			dataService = new AIDataService(this.config);
		}

		public Task<AIResponse> TextRequestAsync(string text)
		{
			if (string.IsNullOrEmpty(text)) {
				throw new ArgumentNullException("text");
			}

			return TextRequestAsync(new AIRequest(text));
		}

		public Task<AIResponse> TextRequestAsync(AIRequest request)
		{
			if (request == null) {
				throw new ArgumentNullException("request");
			}

			return dataService.RequestAsync(request);
		}

        public Task<AIResponse> TextRequestAsync(string text, RequestExtras requestExtras)
        {
            if (string.IsNullOrEmpty(text)) {
                throw new ArgumentNullException("text");
            }

            return TextRequestAsync(new AIRequest(text, requestExtras));
        }

		public Task<AIResponse> VoiceRequestAsync(Stream voiceStream, RequestExtras requestExtras = null)
		{
		    if (config.Language == SupportedLanguage.Italian)
		    {
		        throw new AIServiceException("Sorry, but Italian language now is not supported in Speaktoit recognition. Please use some another speech recognition engine.");
		    }

			return dataService.VoiceRequestAsync(voiceStream, requestExtras);
		}

		public Task<AIResponse> VoiceRequestAsync(float[] samples)
		{
			try {

				var trimmedSamples = TrimSilence(samples);
			
				if (trimmedSamples != null) {
				
					var pcm16 = ConvertIeeeToPcm16(trimmedSamples);
					var bytes = ConvertArrayShortToBytes(pcm16);

					var voiceStream = new MemoryStream(bytes);
					voiceStream.Seek(0, SeekOrigin.Begin);
				
					var aiResponse = VoiceRequestAsync(voiceStream);
					return aiResponse;
				}

			} catch (AIServiceException) {
				throw;
			} catch (Exception e) {
				throw new AIServiceException(e);
			}

			return null;
		}

		
	}
}