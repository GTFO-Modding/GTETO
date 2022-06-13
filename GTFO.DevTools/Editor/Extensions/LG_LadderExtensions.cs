using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace GTFO.DevTools.Extensions
{
    public static class LG_LadderExtensions
    {
        private static readonly Type LADDER_TYPE = typeof(LG_Ladder);
        private static readonly FieldInfo COLLIDER_FIELD = LADDER_TYPE.GetField("m_collider", BindingFlags.Instance | BindingFlags.NonPublic);

        public static BoxCollider GetCollider(this LG_Ladder ladder)
        {
            return COLLIDER_FIELD.GetValue(ladder) as BoxCollider;
        }
        public static void SetCollider(this LG_Ladder ladder, BoxCollider collider)
        {
            COLLIDER_FIELD.SetValue(ladder, collider);
        }
    }
}
