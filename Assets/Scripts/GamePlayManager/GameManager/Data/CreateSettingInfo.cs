using System;

using BC.ODCC;
using BC.OdccBase;

using UnityEngine;

namespace BC.GamePlayManager
{
	public class CreateSettingInfo : DataObject, IFireunitData
	{
		public Vector3Int memberUniqueID;
		public FactionSettingInfo factionSettingInfo;
		public TeamSettingInfo teamSettingInfo;
		public UnitSettingInfo unitSettingInfo;
		public CharacterSettingInfo characterSettingInfo;

		public CreateSettingInfo(MainPlaySetting gameSetting, IFireunitData fireunitData)
		{
			this.memberUniqueID = fireunitData.MemberUniqueID;
			factionSettingInfo = gameSetting.FactionSetting.factionSettingList.Find(i => fireunitData.IsEqualsFaction(i));
			teamSettingInfo = gameSetting.TeamSetting.teamSettingList.Find(i => fireunitData.IsEqualsTeam(i));
			unitSettingInfo = gameSetting.UnitSetting.unitSettingList.Find(i => fireunitData.IsEqualsUnit(i));
			characterSettingInfo = gameSetting.CharacterSetting.characterSettingList[unitSettingInfo.CharacterSetterIndex];

			endSetupFireunit =  false;
			endSetupCharacter = false;
		}

		public Vector3Int MemberUniqueID => memberUniqueID;
		public int FactionIndex { get => memberUniqueID.x; set => memberUniqueID.x = value; }
		public int TeamIndex { get => memberUniqueID.y; set => memberUniqueID.y = value; }
		public int UnitIndex { get => memberUniqueID.z; set => memberUniqueID.z = value; }



		private bool endSetupFireunit = false;
		private bool endSetupCharacter = false;
		private Action deleteAction;

		public void EndSetupFireunit()
		{
			endSetupFireunit = true;
			CheckDelete();
		}
		public void EndSetupCharacter()
		{
			endSetupCharacter = true;
			CheckDelete();
		}
		public void WaitEndSetup(Action deleteAction)
		{
			this.deleteAction = deleteAction;
			endSetupFireunit =  false;
			endSetupCharacter = false;
		}
		private void CheckDelete()
		{
			if(endSetupFireunit && endSetupCharacter)
			{
				deleteAction?.Invoke();
			}
		}
	}
}
