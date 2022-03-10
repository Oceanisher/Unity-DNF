using System;
using System.Collections.Generic;
using Obj;
using Obj.Config.Action;
using Obj.Config.Action.Structure;
using Tools;
using UnityEngine;
using UnityEngine.UI;

namespace Sys.Module.UI
{
    //测试Bar-行为
    public class TestActionBar : MonoSingleton<TestActionBar>
    {
        [Header("行为主类型")]
        [SerializeField]
        private Dropdown actionTypeDd;
        
        [Header("行为子类型")]
        [SerializeField]
        private Dropdown actionTypeDd2;
        
        [Header("行为帧")]
        [SerializeField]
        private Dropdown actionFrame;

        [Header("确认按钮")]
        [SerializeField]
        private Button confirmBtn;
        
        [Header("取消按钮")]
        [SerializeField]
        private Button cancelBtn;

        //物体核心
        private ObjCore _objCore;
        //选中的行为
        private ActionSo _action;

        private void OnEnable()
        {
            SetActionTypeDd();
            SetActionTypeDd2();
            SetActionFrame(_action);
        }

        public void Init(ObjCore core)
        {
            _objCore = core;
            _action = null;
            actionTypeDd.onValueChanged.AddListener(OnActionTypeChange);
            actionTypeDd2.onValueChanged.AddListener(OnActionType2Change);
            confirmBtn.onClick.AddListener(OnBtnConfirm);
            cancelBtn.onClick.AddListener(OnBtnCancel);
        }

        private void OnActionTypeChange(int index)
        {
            ActionType type = (ActionType)Enum.GetValues(typeof(ActionType)).GetValue(index);
            ActionSubType subType = (ActionSubType)Enum.GetValues(typeof(ActionSubType)).GetValue(actionTypeDd2.value);
            if (type == ActionType.None || CollectionUtil.IsEmpty(_objCore.AllActions))
            {
                SetActionFrame(null);
            }
            else
            {
                SetActionFrame(GetAction(type, subType));
            }
        }
        
        private void OnActionType2Change(int index)
        {
            ActionType type = (ActionType)Enum.GetValues(typeof(ActionType)).GetValue(actionTypeDd.value);
            ActionSubType subType = (ActionSubType)Enum.GetValues(typeof(ActionSubType)).GetValue(index);
            if (type == ActionType.None || CollectionUtil.IsEmpty(_objCore.AllActions))
            {
                SetActionFrame(null);
            }
            else
            {
                SetActionFrame(GetAction(type, subType));
            }
        }

        private void OnBtnConfirm()
        {
            if (null != _action)
            {
                //-1是因为开头增加了一个-1
                _objCore.ActionChange_Ai(_action, actionFrame.value - 1);
            }
        }
        
        private void OnBtnCancel()
        {
            _objCore.ActionChange_Manual();
        }

        //设置行为主类型
        private void SetActionTypeDd()
        {
            List<string> strList = new List<string>();
            foreach (ActionType item in Enum.GetValues(typeof(ActionType)))
            {
                strList.Add(item.ToString());
            }
            actionTypeDd.ClearOptions();
            actionTypeDd.AddOptions(strList);
            actionTypeDd.value = 0;
        }

        //设置行为子类型
        private void SetActionTypeDd2()
        {
            List<string> strList = new List<string>();
            foreach (ActionSubType item in Enum.GetValues(typeof(ActionSubType)))
            {
                strList.Add(item.ToString());
            }
            actionTypeDd2.ClearOptions();
            actionTypeDd2.AddOptions(strList);
            actionTypeDd2.value = 0;
        }

        //设置行为帧
        private void SetActionFrame(ActionSo action)
        {
            //设置行为
            _action = action;
            
            if (null == action)
            {
                actionFrame.ClearOptions();
                return;
            }
            
            List<string> strList = new List<string>();
            //添加-1，表示循环该动作
            strList.Add(ActionConstant.InvalidFrameIndex.ToString());
            for (int i = 0; i < action.actionFrame.aniFrameInfos.Count - 1 ; i++)
            {
                strList.Add(i.ToString());
            }
            actionFrame.ClearOptions();
            actionFrame.AddOptions(strList);
            actionFrame.value = 0;
        }

        //获取行为
        private ActionSo GetAction(ActionType type, ActionSubType subType)
        {
            foreach (var item in _objCore.AllActions)
            {
                if (item.type == type 
                    && subType == ActionSubType.None)
                {
                    return item;
                }

                if (item.type == type
                    && subType != ActionSubType.None
                    && subType == item.subType)
                {
                    return item;
                }
            }

            return null;
        }
    }
}