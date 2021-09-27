using CitizenFX.Core;
using MenuAPI;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace vorpstores_cl.Menus
{
    public class BuyMenu
    {
        private static Menu buyMenu = new Menu(GetConfig.Langs["BuyButton"], GetConfig.Langs["BuyMenuDesc"]);
        private static Menu buyMenuConfirm = new Menu("", GetConfig.Langs["BuyMenuConfirmDesc"]);

        private static int indexItem;
        private static int quantityItem;

        private static bool setupDone = false;

        public static List<string> quantityList = new List<string>();

        private static void SetupMenu(JObject AllcurrentPrices)
        {
            if (setupDone) return;
            setupDone = true;
            MenuController.AddMenu(buyMenu);

            MenuController.EnableMenuToggleKeyOnController = false;
            MenuController.MenuToggleKey = (Control)0;

            MenuController.AddSubmenu(buyMenu, buyMenuConfirm);

            for (var i = 1; i < 101; i++)
            {
                quantityList.Add($"{GetConfig.Langs["Quantity"]} #{i}");
            }

            MenuItem subMenuConfirmBuyBtnYes = new MenuItem("", " ")
            {
                RightIcon = MenuItem.Icon.TICK
            };
            MenuItem subMenuConfirmBuyBtnNo = new MenuItem(GetConfig.Langs["BuyConfirmButtonNo"], " ")
            {
                RightIcon = MenuItem.Icon.ARROW_LEFT
            };

            buyMenuConfirm.AddMenuItem(subMenuConfirmBuyBtnYes);
            buyMenuConfirm.AddMenuItem(subMenuConfirmBuyBtnNo);

            buyMenu.OnListItemSelect += (_menu, _listItem, _listIndex, _itemIndex) =>
            {

                /* foreach AllCurrentPrices search for ItemName
                 * GetConfig.Config["Stores"][StoreActions.LaststoreId]["ItemsBuy"][_itemIndex]["Name"].ToString();
                 */

                indexItem = _itemIndex;
                quantityItem = _listIndex + 1;
                double totalPrice = double.Parse(GetConfig.Config["Stores"][StoreActions.LaststoreId]["ItemsBuy"][_itemIndex]["BuyPrice"].ToString()) * quantityItem;
                buyMenuConfirm.MenuTitle = GetConfig.ItemsFromDB[GetConfig.Config["Stores"][StoreActions.LaststoreId]["ItemsBuy"][_itemIndex]["Name"].ToString()]["label"].ToString();
                subMenuConfirmBuyBtnYes.Label = string.Format(GetConfig.Langs["BuyConfirmButtonYes"], (_listIndex + 1).ToString(), GetConfig.ItemsFromDB[GetConfig.Config["Stores"][StoreActions.LaststoreId]["ItemsBuy"][_itemIndex]["Name"].ToString()]["label"].ToString(), totalPrice.ToString());
            };

            buyMenu.OnIndexChange += (_menu, _oldItem, _newItem, _oldIndex, _newIndex) =>
            {
                StoreActions.CreateObjectOnTable(_newIndex, "ItemsBuy");
            };

            buyMenu.OnMenuOpen += (_menu) =>
            {
                buyMenu.ClearMenuItems();

                foreach (var item in GetConfig.Config["Stores"][StoreActions.LaststoreId]["ItemsBuy"])
                {

                    string newPrice = "failed";

                    foreach (var index in AllcurrentPrices)
                    {
                        if (JObject.Parse(index.Value.ToString())["item"].ToString() == item["Name"].ToString())
                        {
                            newPrice = JObject.Parse(index.Value.ToString())["buy"].ToString();
                        }
                    }


                    /*
                     * Something like that may be needed here instead of "MenuListItem _itemToBuy = new MenuListItem..."
                     * 
                        MenuDynamicListItem playerOutfit = new MenuDynamicListItem("Select Outfit", "0", new MenuDynamicListItem.ChangeItemCallback((item, left) =>
                        {
                            if (int.TryParse(item.CurrentItem, out int val))
                            {
                                int newVal = val;
                                if (left)
                                {
                                    newVal--;
                                    if (newVal < 0)
                                    {
                                        newVal = 0;
                                    }
                                }
                                else
                                {
                                    newVal++;
                                }
                                SetPedOutfitPreset(PlayerPedId(), newVal, 0);
                                return newVal.ToString();
                            }
                            return "0";
                        }), "Select a predefined outfit for this ped. Outfits are made by Rockstar. Note the selected value can go up indefinitely because we don't know how to check for the max amount of outfits yet, so more native research is needed.");
                    */


                    //MenuListItem _itemToBuy = new MenuListItem(GetConfig.ItemsFromDB[item["Name"].ToString()]["label"].ToString() + $" ${item["BuyPrice"]}", quantityList, 0, "")
                    MenuListItem _itemToBuy = new MenuListItem(GetConfig.ItemsFromDB[item["Name"].ToString()]["label"].ToString() + $" ${newPrice}", quantityList, 0, "")
                    {

                    };

                    buyMenu.AddMenuItem(_itemToBuy);
                    MenuController.BindMenuItem(buyMenu, buyMenuConfirm, _itemToBuy);
                }

                StoreActions.CreateObjectOnTable(_menu.CurrentIndex, "ItemsBuy");
            };

            buyMenuConfirm.OnItemSelect += (_menu, _item, _index) =>
            {
                if (_index == 0)
                {
                    StoreActions.BuyItemStore(indexItem, quantityItem);
                    buyMenu.OpenMenu();
                    buyMenuConfirm.CloseMenu();
                }
                else
                {
                    buyMenu.OpenMenu();
                    buyMenuConfirm.CloseMenu();
                }
            };

        }

        public static Menu GetMenu(JObject AllcurrentPrices)
        {
            SetupMenu(AllcurrentPrices);
            return buyMenu;
        }
    }
}