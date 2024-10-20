using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using BC.OdccBase;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.GamePlayManager
{
	public class FactionSetting : SubPlaySetting
	{
		[InfoBox("Duplicate found in the list.", InfoMessageType.Error, "HasDuplicate")]
		[ListDrawerSettings(CustomAddFunction = "OnAddSetting")]
		public List<FactionSettingInfo> factionSettingList;
#if UNITY_EDITOR
		private FactionSettingInfo OnAddSetting()
		{
			// 새로운 유닛 설정 항목을 생성합니다.
			FactionSettingInfo newSetting = new FactionSettingInfo();

			// 현재 리스트에서 마지막 유닛 인덱스를 가져와 +1하여 새로운 유닛 설정에 할당합니다.
			int lastFactIndex = factionSettingList.Count > 0 ? factionSettingList[^1].FactionIndex + 1 : 0;
			newSetting.FactionIndex = lastFactIndex;

			// 새로운 유닛 설정 항목을 반환합니다.
			return newSetting;
		}
		private bool HasDuplicate() => factionSettingList != null && factionSettingList.GroupBy(u => u.FactionIndex).Any(g => g.Count() > 1);
		[Button, PropertyOrder(-1)]
		private void SortList()
		{
			if(factionSettingList == null) return;
			factionSettingList = factionSettingList
				.OrderBy(u => u.FactionIndex) // FactionIndex 기준으로 먼저 정렬
				.ToList();
		}
#endif
	}
	[Serializable]
	public struct FactionSettingInfo : IFactionIndex
	{
		[SerializeField, HideLabel,SuffixLabel("Faction Color   _", Overlay = true), HorizontalGroup("Color")]
		private Color factionColor;
		[SerializeField, EnumToggleButtons, HideLabel]
		private FactionControlType factionControlType;
		[HorizontalGroup("ID"), HideLabel, SuffixLabel("Index", Overlay = true), SerializeField]
		private int factionIndex;
		[HorizontalGroup("ID"), HideLabel, SuffixLabel("Name", Overlay = true), SerializeField]
		private string factionName;
		[SerializeField, LabelText("Diplomacy")]
		private List<DiplomacySettingInfo> diplomacyInfoList;

		public FactionControlType FactionControl { get => factionControlType; set => factionControlType=value; }
		public int FactionIndex { get => factionIndex; set => factionIndex=value; }
		public string FactionName { get => factionName; set => factionName=value; }
		public Color FactionColor { get => factionColor; set => factionColor=value; }
		public List<DiplomacySettingInfo> DiplomacyInfoList { get => diplomacyInfoList; set => diplomacyInfoList=value; }
#if UNITY_EDITOR
		private Texture2D selectedTexture { get; set; }
		[ValueDropdown("GetColorTextures"), ShowInInspector, HideLabel, HorizontalGroup("Color", width: 55)]
		private Texture2D _selectedTexture {
			get {
				if(selectedTexture == null)
				{
					selectedTexture = new Texture2D(0, 0);
					selectedTexture.name = "Select";
					return selectedTexture;
				}
				else
				{
					return selectedTexture;
				}
			}
			set {
				selectedTexture = value;
				if(value != null)
				{
					factionColor = value.GetPixel(0, 0);
				}
			}
		}
		private List<Texture2D> colorTexturePreview { get; set; }
		private IEnumerable GetColorTextures()
		{
			ValueDropdownList<Texture2D> list = new ValueDropdownList<Texture2D>();
			var colors = Sirenix.OdinInspector.Editor.ColorPaletteManager.Instance.ColorPalettes
					.Where(x => x.Name == "Faction Color")
					.SelectMany(x => x.Colors);

			if(colorTexturePreview == null || colorTexturePreview.Count == 0)
			{
				colorTexturePreview = new List<Texture2D>();
				{
					foreach(var item in colors)
					{
						Texture2D texture = new Texture2D(20, 20);
						for(int x = 0 ; x < texture.width ; x++)
						{
							for(int y = 0 ; y < texture.height ; y++)
							{
								texture.SetPixel(x, y, item);
							}
						}
						texture.Apply();
						colorTexturePreview.Add(texture);
					}
				}
			}

			int Count = colorTexturePreview.Count;
			for(int i = 0 ; i < Count ; i++)
			{
				list.Add($"Select : {i:00}", colorTexturePreview[i]);
			}
			return list;
		}

#endif
		public void Dispose() { }
	}
	[Serializable]
	public struct DiplomacySettingInfo
	{
		[SerializeField]
		[HideLabel, HorizontalGroup(width:100), ValueDropdown("FactionTarget_ValueDropdown")]
		private int factionTarget;

		[SerializeField, EnumToggleButtons]
		[HideLabel, HorizontalGroup]
		private FactionDiplomacyType factionDiplomacy;

		public int FactionTarget { get => factionTarget; set => factionTarget=value; }
		public FactionDiplomacyType FactionDiplomacy { get => factionDiplomacy; set => factionDiplomacy=value; }


#if UNITY_EDITOR
		private IEnumerable FactionTarget_ValueDropdown()
		{
			ValueDropdownList<int> list = new ValueDropdownList<int>();
			if(UnityEditor.Selection.activeObject is FactionSetting factionSetting)
			{
				int length = factionSetting.factionSettingList.Count;
				for(int i = 0 ; i < length ; i++)
				{
					string factionName = factionSetting.factionSettingList[i].FactionName;
					int factionIndex = factionSetting.factionSettingList[i].FactionIndex;
					if(string.IsNullOrWhiteSpace(factionName))
					{
						list.Add(factionIndex.ToString(), factionIndex);
					}
					else
					{
						list.Add(factionName, factionIndex);
					}
				}
			}
			return list;
		}
#endif
	}
}
