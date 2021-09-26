using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace vorpstores_cl
{
    class GetDynamics : BaseScript
    {
        public static JObject currentPrices = new JObject();
        public static JObject AllcurrentPrices = new JObject();
        public static bool updateCP = false;

        public GetDynamics()
        {
            EventHandlers[$"{API.GetCurrentResourceName()}:sendValues"] += new Action<string>(GetValues);
            EventHandlers[$"{API.GetCurrentResourceName()}:callServerEventSingle"] += new Action<string>(CallServerEventSingle);
            EventHandlers[$"{API.GetCurrentResourceName()}:callServerEventAll"] += new Action(CallServerEventAll);
        }

        public static async Task GetSingleCurrentPrices(string itemName)
        {
            TriggerEvent($"{API.GetCurrentResourceName()}:callServerEventSingle", itemName);
            while (currentPrices.Count <= 0 || !updateCP)
            {
                await Delay(75);
            }

            updateCP = false;
        }

        public static async Task GetAllCurrentPrices()
        {
            TriggerEvent($"{API.GetCurrentResourceName()}:callServerEventAll");
            while (AllcurrentPrices.Count <= 0 || !updateCP)
            {
                await Delay(75);
            }

            updateCP = false;
        }

        private void GetValues(string cp)
        {
            /*foreach (var l in dl)
            {
                currentPrices[l.Key] = l.Value.ToString();
            }*/

            var json = JObject.Parse(cp);

            if (json.Count > 1)
            {
                AllcurrentPrices = json;

            } else
            {
                currentPrices = json;
            }

            updateCP = true;
        }

        private void CallServerEventSingle(string itemName)
        {
            TriggerServerEvent($"{API.GetCurrentResourceName()}:getSingleValues", itemName);
        }

        private void CallServerEventAll()
        {
            TriggerServerEvent($"{API.GetCurrentResourceName()}:getAllValues");
        }
    }
}
