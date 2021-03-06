﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Helper
{
    public class ApiClient
    {

        private ApiSetting _apiSetting;
        private HttpClient _client;
        private string _apiController;
        private string _apiAction;
        public ApiClient(ApiSetting apiSetting)
        {
            this._apiSetting = apiSetting;
        }

        public void InitializeClient(string webApiCaller)
        {
            string[] stringSeparators = new string[] { "_" };
            this._apiController = webApiCaller.Split(stringSeparators, StringSplitOptions.None)[0];
            this._apiAction = webApiCaller.Split(stringSeparators, StringSplitOptions.None)[1];
            this._client = new HttpClient();
            //Passing service base url    
            this._client.BaseAddress = new Uri(this._apiSetting.WebApiUrl.ToString());

            this._client.DefaultRequestHeaders.Clear();
            //Define request data format    
            this._client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private string MakeApiUri(List<ApiParameter> parameters = null)
        {
            ApiController webApiController = this._apiSetting.WebApiControllers.Find(x => x.Name == this._apiController);
            ApiAction webApiAction = webApiController.WebApiActions.Find(x => x.Name == this._apiAction);
            //List<WebApiParameter> parameters = webApiAction.Parameters;
            string returnValue = "";
            returnValue += this._apiSetting.WebApiPrefix;
            returnValue += "/" + webApiController.Name;
            returnValue += "/" + webApiAction.Name;
            if (parameters != null)
            {
                string parameterValue = "";
                if (webApiAction.IsCustom)
                {
                    for (int i = 0; i < parameters.Count; i++)
                    {
                        if ("".Equals(parameterValue))
                        {
                            parameterValue += "?" + parameters[i].Name + "=" + parameters[i].Value;
                        }
                        else
                        {
                            parameterValue += "&" + parameters[i].Name + "=" + parameters[i].Value;
                        }
                    }
                    returnValue += parameterValue;
                }
                else
                {
                    returnValue += "/" + parameters[0].Value;
                }

            }
            return returnValue;
        }

        /*private string MakeApiUri(WebApiParameter parameter = null){
            WebApiCaller webApiCaller = this._apiSetting.WebApiCallers.Find(x => x.Name == this._apiCaller);
            WebApiController webApiController = webApiCaller.Controller;
            WebApiAction webApiAction = webApiController.Action;
            //List<WebApiParameter> parameters = webApiAction.Parameters;
            string returnValue = "";
            returnValue += this._apiSetting.WebApiPrefix;
            returnValue += "/" + webApiController.Name;
            returnValue += "/" + webApiAction.Name;
            if( parameter != null){
                returnValue += "/" + parameter.Value;
            }            
            return returnValue;
        }*/

        /*private string MakePostApi(){
            WebApiCaller webApiCaller = this._apiSetting.WebApiCallers.Find(x => x.Name == this._apiCaller);
            WebApiController webApiController = webApiCaller.Controller;
            WebApiAction webApiAction = webApiController.Action;
            List<WebApiParameter> parameters = webApiAction.Parameters;
            string returnValue = "";
            returnValue += this._apiSetting.WebApiPrefix;
            returnValue += "/" + webApiController.Name;
            returnValue += "/" + webApiAction.Name;
            returnValue += parameterValue;
            return returnValue;
        }*/

        public async Task<List<T>> List<T>(List<ApiParameter> parameters = null)
        {
            HttpResponseMessage res = await this._client.GetAsync(MakeApiUri(parameters));

            //Checking the response is successful or not which is sent using HttpClient    
            if (res.IsSuccessStatusCode)
            {
                //Storing the response details recieved from web api     
                var result = res.Content.ReadAsStringAsync().Result;

                //Deserializing the response recieved from web api and storing into the Employee list    
                return JsonConvert.DeserializeObject<List<T>>(result);

            }
            return default(List<T>);
        }

        public async Task<T> Get<T>(List<ApiParameter> parameters = null)
        {
            HttpResponseMessage res = await this._client.GetAsync(MakeApiUri(parameters));
            //Checking the response is successful or not which is sent using HttpClient    
            if (res.IsSuccessStatusCode)
            {
                //Storing the response details recieved from web api     
                var result = res.Content.ReadAsStringAsync().Result;

                //Deserializing the response recieved from web api and storing into the Employee list    
                return JsonConvert.DeserializeObject<T>(result);

            }
            return default(T);
        }
        /*
        public async Task<List<T>> ListFromApi<T>(WebApiParameter parameter = null){
            HttpResponseMessage res = await this._client.GetAsync(MakeApiUri(parameter));  

            //Checking the response is successful or not which is sent using HttpClient    
            if (res.IsSuccessStatusCode)  
            {  
                //Storing the response details recieved from web api     
                var result = res.Content.ReadAsStringAsync().Result;  
  
                //Deserializing the response recieved from web api and storing into the Employee list    
                return JsonConvert.DeserializeObject<List<T>>(result);  
  
            }
            return default(List<T>);
        }

        public async Task<T> GetFromApi<T>(WebApiParameter parameter = null){
            HttpResponseMessage res = await this._client.GetAsync(MakeApiUri(parameter));  
            //Checking the response is successful or not which is sent using HttpClient    
            if (res.IsSuccessStatusCode)  
            {  
                //Storing the response details recieved from web api     
                var result = res.Content.ReadAsStringAsync().Result;  
  
                //Deserializing the response recieved from web api and storing into the Employee list    
                return JsonConvert.DeserializeObject<T>(result);  
  
            }
            return default(T);
        }*/

        public async Task<T> Post<T>(T postItem)
        {
            string stringData = JsonConvert.SerializeObject(postItem);
            var contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage res = await this._client.PostAsync(MakeApiUri(), contentData);
            //res.EnsureSuccessStatusCode();
            var returnVal = res.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<T>(returnVal);
        }

        public async Task<T> Put<T>(List<ApiParameter> parameters, T putItem)
        {
            string stringData = JsonConvert.SerializeObject(putItem);
            var contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage res = await this._client.PutAsync(MakeApiUri(parameters), contentData);
            //res.EnsureSuccessStatusCode();
            var returnVal = res.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<T>(returnVal);
        }

        public async Task<string> Delete(List<ApiParameter> parameters)
        {
            HttpResponseMessage res = await this._client.DeleteAsync(MakeApiUri(parameters));
            //res.EnsureSuccessStatusCode();
            var returnVal = res.Content.ReadAsStringAsync().Result;
            return returnVal.ToString();
        }
    }
}
