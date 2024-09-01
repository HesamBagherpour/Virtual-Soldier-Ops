using System.Collections.Generic;
using System.Linq;
using ArioSoren.UIKit.Core;
using UnityEngine;

namespace ArioSoren.VirtualCarOps.General_Utility.DataTypes.GUI
{
    [CreateAssetMenu(fileName = "Data", menuName = "DataTypes/GUI/PageUI")]
    public class PageContainer : ScriptableObject
    {
        public List<PageBaseUI> listPageUi = new();

        public PageBaseUI GetPage(PageType type)
        {
            var page = listPageUi.FirstOrDefault(p => p.Type == type);

            if (page is null) Debug.LogError($"There is no page with name {name}");

            return page;
        }
    }
}