using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace vorpstores_sv
{
    class GetDynamics : BaseScript
    {

        public GetDynamics()
        {
            EventHandlers[$"{API.GetCurrentResourceName()}:getSingleValues"] += new Action<Player, string>(GetSingleValues);
            EventHandlers[$"{API.GetCurrentResourceName()}:getAllValues"] += new Action<Player>(GetAllValues);
            EventHandlers[$"{API.GetCurrentResourceName()}:test_sv"] += new Action(Test);
        }

        private void GetSingleValues([FromSource]Player source, string itemName)
        {
            string SingleCurrentPrices = JsonConvert.SerializeObject(Exports["MM_dynamicPrices"].getSingleCurrentPrices(itemName));
            source.TriggerEvent($"{API.GetCurrentResourceName()}:sendValues", SingleCurrentPrices);
        }

        private void GetAllValues([FromSource] Player source)
        {
            string AllCurrentPrices = JsonConvert.SerializeObject(Exports["MM_dynamicPrices"].getAllCurrentPrices());
            source.TriggerEvent($"{API.GetCurrentResourceName()}:sendValues", AllCurrentPrices);
        }

        private void Test()
        {
            //string json = JsonConvert.SerializeObject(Exports["MM_dynamicPrices"].getAllCurrentPrices());

            //only JObject.Parse() at getAllCurrentPrices()
            //var AllCurrentPrices = JObject.Parse(json);
            var AllCurrentPrices = JObject.Parse(JsonConvert.SerializeObject(Exports["MM_dynamicPrices"].getAllCurrentPrices()));

            foreach (var i in AllCurrentPrices)
            {
                //JObject.Parse(i.Value.ToString())
                if (JObject.Parse(i.Value.ToString())["item"].ToString() == "Yarrow_Seed")
                {
                    Debug.WriteLine(i.Value.ToString());
                } else
                {
                    Debug.WriteLine("no");
                }
            }

            Debug.WriteLine("DEBUG COUNT: " + AllCurrentPrices.Count);

            //Debug.WriteLine(json_obj.ToString());

            //source.TriggerEvent($"{API.GetCurrentResourceName()}:sendValues", Exports["MM_dynamicPrices"].getAllCurrentPrices());



            /*var SingleCurrentPrices = JObject.Parse(JsonConvert.SerializeObject(Exports["MM_dynamicPrices"].getSingleCurrentPrices("Yarrow_Seed")));
            Debug.WriteLine(SingleCurrentPrices.ToString());*/
            //source.TriggerEvent($"{API.GetCurrentResourceName()}:sendValues", SingleCurrentPrices);

        }
    }
}
