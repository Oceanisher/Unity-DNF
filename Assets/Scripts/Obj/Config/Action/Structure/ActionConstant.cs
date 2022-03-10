namespace Obj.Config.Action.Structure
{
    //行为静态变量
    public static class ActionConstant
    {
        //无效帧序列标识
        public static readonly int InvalidFrameIndex = -1;
        //无效高度百分比标识
        public static readonly int InvalidFallPercent = -1;

        //通用间隔符
        public static readonly string SplitSymbol = "/";

        //Hierarchy、Project中Body、皮肤相关Object的名称-图形根节点Graphics
        public static readonly string Graphics = "Graphics";
        //Hierarchy、Project中Body、皮肤相关Object的名称-Body
        public static readonly string Body = "Body";
        //Hierarchy、Project中Body、皮肤相关Object的名称-所有皮肤的父节点
        public static readonly string Skin = "Skin";
        //Hierarchy、Project中Body、皮肤相关Object的名称-SkinHair
        public static readonly string SkinHair = "Hair";
        //Hierarchy、Project中Body、皮肤相关Object的名称-SkinChest
        public static readonly string SkinChest = "Chest";
        //Hierarchy、Project中Body、皮肤相关Object的名称-SkinShoots
        public static readonly string SkinShoots = "Shoots";
        //Hierarchy、Project中Body、皮肤相关Object的名称-SkinShoots2
        public static readonly string SkinShoots2 = "Shoots_2";
        //Hierarchy、Project中Body、皮肤相关Object的名称-SkinLegs
        public static readonly string SkinLegs = "Legs";
        //Hierarchy、Project中Body、皮肤相关Object的名称-SkinLegs2
        public static readonly string SkinLegs2 = "Legs_2";
        //Hierarchy、Project中Body、皮肤相关Object的名称-SkinWeapon
        public static readonly string SkinWeapon = "Weapon";
        //Hierarchy、Project中Body、皮肤相关Object的名称-SkinWeapon2
        public static readonly string SkinWeapon2 = "Weapon_2";
        
        //Hierarchy、Project中物理相关Object的名称-物理根节点Physics
        public static readonly string Physics = "Physics";
        //Hierarchy、Project中物理相关Object的名称-动态碰撞器根节点
        public static readonly string DynamicCollider = "Dynamic_Collider";
        //Hierarchy、Project中物理相关Object的名称-动态碰撞器物体-XZ轴
        public static readonly string DynamicColliderXZ = "XZ";
        //Hierarchy、Project中物理相关Object的名称-动态碰撞器物体-Y轴
        public static readonly string DynamicColliderY = "Y";
        //Hierarchy、Project中物理相关Object的名称-静态碰撞器物体-XZ轴
        public static readonly string StaticColliderXZ = "XZ_Collider";
    }
}