using System.Collections.Generic;
using System.Linq;
using ArioSoren.UIKit.Core;
using UnityEngine;

namespace ArioSoren.VirtualCarOps.General_Utility.DataTypes.GUI
{
    [CreateAssetMenu(fileName = "DataAnimation", menuName = "DataTypes/GUI/DataAnimation")]
    public class AnimationData : ScriptableObject
    {
        public List<AnimationBaseUI> listAnimationData = new();

        public AnimationBaseUI GetAnimationType(CurrentAnimationState type)
        {
            var page = listAnimationData.FirstOrDefault(p => p.type == type);
            if (page is null) Debug.LogError($"There is no page with name {name}");

            return page;
        }
    }
}