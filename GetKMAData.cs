using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using System.ComponentModel;

namespace LGUPlus
{
    [Category("LGUplus")]
    [DisplayName("Call KMA API")]
    [Description("KMA에서 제공하는 데이터 가져오기")]
    public class GetKMAData : CodeActivity
    {

        [Category("Input")]
        [RequiredArgument]
        [DisplayName("KMA API endpoint - 전체 URL")]
        public InArgument<string> Endpoint { get; set; }
        [Category("Output")]
        [RequiredArgument]
        [DisplayName("KMA API 호출 결과")]
        public OutArgument<string> Result { get; set; }

        private UiPathHttpClient _httpClient;
        private KMAResponse _response;

        public GetKMAData()
        {
            _httpClient = new UiPathHttpClient();
        }

        protected override async void Execute(CodeActivityContext context)
        {
            var endpoint = Endpoint.Get(context);

            _httpClient.setEndpoint(endpoint);
            _response = await _httpClient.Download();
            if( _response.status == System.Net.HttpStatusCode.OK )
            {
                Result.Set(context, _response.body);
            }
            else
            {
                Result.Set(context, string.Empty);
            }
        }
    }
}
