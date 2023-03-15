using System.Collections.Generic;
using UnityEngine;
using Utils.Input;

namespace UI
{
    [CreateAssetMenu(fileName = "SchemeSpriteAtlasProvider", menuName = "SchemeSpriteAtlasProvider", order = 0)]
    public class SchemeSpriteAtlasProvider : ScriptableObject
    {
        private static readonly Dictionary<string, string> SchemesToAtlasMap = new Dictionary<string, string>
        {
            { "Gamepad", "xbox" },
            { "Keyboard&Mouse", "desktop" }
        };

        public string SchemeAtlas => SchemesToAtlasMap[InputSchemeObserverAsset.Instance.CurrentScheme.name];
    }
}