using System.Collections.Generic;
using Sys.Module;

namespace Tools
{
    //按键处理接口
    public interface IKeyHandler
    {
        //处理按键
        public void HandleKeys(List<InputManager.KeyInfo> keyInfos);
    }
}