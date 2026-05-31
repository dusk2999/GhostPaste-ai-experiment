# GhostPaste UI 交接文档

## 项目概述

GhostPaste 是一个模拟键盘输入实现粘贴的 Windows 小工具。通过 `SendInput` API 逐字符发送 Unicode 文本到目标窗口，绕过禁止粘贴的限制。

当前 UI 是功能验证用的临时界面，需要重新设计。

---

## 技术栈

- .NET 8 / WPF
- 无第三方 UI 库（可按需引入）
- 单窗口应用，无边框 (`WindowStyle="None"`, `AllowsTransparency="True"`)
- 系统 Mica 毛玻璃效果（通过 `DwmSetWindowAttribute` 实现）

---

## 当前窗口结构

```
┌─────────────────────────────────┐
│ 👻 GhostPaste            [✕]   │  ← 自定义标题栏，可拖动
├─────────────────────────────────┤
│                                 │
│   [多行文本输入框]               │  ← x:Name="InputBox"
│                                 │
│                                 │
├─────────────────────────────────┤
│ ⚡极速 ──────●────── 🐢拟人     │  ← x:Name="SpeedSlider" (0~120ms)
├─────────────────────────────────┤
│         [ 发 送 ]               │  ← x:Name="SendButton"
└─────────────────────────────────┘
```

窗口属性：
- 尺寸：420×340，可拖拽右下角缩放
- 始终置顶 (`Topmost="True"`)
- 背景：`#E8202020` 半透明深色 + 1px `#40FFFFFF` 边框 + 12px 圆角

---

## UI 控件与 code-behind 的绑定关系

| 控件 | x:Name | 类型 | code-behind 如何使用 |
|------|--------|------|---------------------|
| 文本输入框 | `InputBox` | TextBox | 读取 `.Text` 作为发送内容 |
| 速度滑块 | `SpeedSlider` | Slider | 读取 `.Value`（int，单位 ms）作为字符间延迟 |
| 发送按钮 | `SendButton` | Button | 点击触发 `SendButton_Click`；发送中设 `IsEnabled=false`，`Content="发送中..."` |

---

## UI 需要触发的事件/方法

code-behind 中已有以下事件处理器，UI 改版时需保留绑定：

```csharp
// 标题栏拖动
private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)

// 关闭按钮
private void CloseButton_Click(object sender, RoutedEventArgs e)

// 发送按钮
private async void SendButton_Click(object sender, RoutedEventArgs e)
```

---

## 发送流程（UI 需配合的状态变化）

```
用户点击"发送"
    │
    ├─ 文本为空 → 不响应
    ├─ 目标窗口未检测到 → 弹 MessageBox 提示
    │
    └─ 正常流程：
         ① SendButton.IsEnabled = false
         ② SendButton.Content = "发送中..."
         ③ 窗口 Hide()（隐藏自己，避免抢焦点）
         ④ 切换到目标窗口 + 关闭 IME + 逐字发送
         ⑤ 发送完毕：Show() + 恢复按钮状态
```

**如果 UI 想加进度条/动画**，可以在步骤 ④ 中通过 `IProgress<T>` 回调，但当前未实现——需要时告诉我，我加接口。

---

## 目标窗口检测机制

- 用 `DispatcherTimer` 每 300ms 轮询 `GetForegroundWindow()`
- 记录最后一个不是自己的前台窗口句柄
- UI 上可以考虑显示目标窗口状态（如"目标：Chrome"），需要时我可以暴露窗口标题

---

## 毛玻璃效果

`Helpers/GlassHelper.cs` 在 `SourceInitialized` 时调用，对窗口 HWND 设置 Mica 背景。

UI 侧需要注意：
- `Window.Background` 必须是 `Transparent`
- 实际背景色由最外层 `Border.Background` 控制（当前 `#E8202020`）
- 如果想让 Mica 效果更明显，可以降低 Border 背景的 alpha 值（如 `#C0202020`）

---

## 改版时的约束

1. **必须保留的 x:Name**：`InputBox`、`SpeedSlider`、`SendButton`
2. **必须保留的事件绑定**：`TitleBar_MouseLeftButtonDown`、`CloseButton_Click`、`SendButton_Click`
3. **窗口属性不能改**：`WindowStyle="None"`、`AllowsTransparency="True"`、`Topmost="True"`
4. **SpeedSlider 范围**：`Minimum="0" Maximum="120"`，Value 直接当毫秒用
5. **不要引入 MVVM 框架** — 这是个小工具，code-behind 直接操作控件就行

---

## 可以自由发挥的部分

- 整体视觉风格、配色、字体
- 窗口尺寸和比例
- 按钮/滑块的自定义样式
- 动画和过渡效果
- 是否加最小化按钮、置顶切换按钮
- 标题栏布局
- 是否显示目标窗口名称、发送进度、字符计数等辅助信息
- 是否加深色/浅色主题切换

---

## 文件结构

```
GhostPaste/
├── GhostPaste.csproj
├── App.xaml                    # 不需要改
├── App.xaml.cs                 # 不需要改
├── MainWindow.xaml             # ← UI 改这个
├── MainWindow.xaml.cs          # 逻辑层，尽量不动
├── Native/
│   ├── User32.cs              # Win32 P/Invoke，不要动
│   └── InputSimulator.cs      # SendInput 逻辑，不要动
└── Helpers/
    └── GlassHelper.cs         # 毛玻璃，不要动
```

---

## 构建 & 运行

```powershell
# 构建
dotnet build -c Release

# 运行（调试）
dotnet run

# 发布单文件
dotnet publish -c Release -o publish
# 产出：publish/GhostPaste.exe (约 177MB，自包含运行时)
```

---

## 联系

逻辑层有任何需要配合的（暴露新属性、加回调接口、改控件名等），直接说。
