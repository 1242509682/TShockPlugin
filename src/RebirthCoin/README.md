# RebirthCoin 复活币

- 作者: GK & 羽学
- 出处: GK的QQ群232109072 
- 这是一个Tshock服务器插件，主要用于：消耗指定道具，让玩家快速复活。

## 更新日志

```
v1.0.2
适配.Net 6.0
将复活币的物品ID从整数改为数组（支持更多物品作为复活币）
将复活币权限名改为：RebirthCoin
```

## 指令

| 语法                             | 别名  |       权限       |                   说明                   |
| -------------------------------- | :---: | :--------------: | :--------------------------------------: |
| 无  | 无 |   RebirthCoin    |    复活币使用权限    |  
| /reload  | 无 |   tshock.cfg.reload    |    重载配置文件    |  

## 配置
> 配置文件位置：tshock/复活币.json
```json
{
  "插件开关": true, //开启或者关闭插件功能
  "允许PVP复活": false, //允许或禁止玩家在pvp模式下使用复活币
  "复活提醒": "{0} 被圣光笼罩，瞬间复活!!!", //复活时提示语的字符串
  "复活提醒的颜色": [
    255,  //复活提示语的颜色,分别为RGB,数值越大颜色越浅,全部填写255就是白色。
    215,
    0
  ],
  "复活币的物品ID": [
    3229 //具体使用哪些物品作为复活币，这里写的是物品ID，支持多个物品用","逗号隔开
  ]
}
```
## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love