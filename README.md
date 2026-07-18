# GhostPaste AI 实验版

GhostPaste AI 实验版是从 GhostPaste 拆出的独立实验项目。它保留原来的“模拟真实键盘输入来粘贴文本”能力，并在此基础上加入 AI 问答、截图发送、图片预览裁切、Markdown 渲染和记录板。

独立项目地址：

https://github.com/dusk2999/GhostPaste-ai-experiment

## 项目状态

这是一个实验性分支项目，用来测试 AI 问答和截图辅助能力。原始 GhostPaste 项目仍然保留原有的粘贴发送工作流，本仓库用于独立演进实验功能。

## 功能特性

- 模拟键盘粘贴：通过模拟真实按键发送文本，不依赖 `Ctrl+V`。
- 置顶悬浮窗：窗口保持在其他软件上方，便于随时操作。
- 透明界面：透明度滑条可以让窗口背景接近完全透明，同时保留文字和控件。
- 可调发送速度：支持快速发送，也支持更接近真人敲字的慢速输入。
- 目标窗口追踪：自动记录最近激活的非 GhostPaste 窗口作为发送目标。
- AI 问答：支持调用 OpenAI 兼容的 Responses API。
- 运行时 AI 设置：调用地址和密钥在界面中配置，不写死在源码里。
- 截图辅助提问：支持全屏截图、目标窗口截图和 `PrintWindow` 截图。
- 图片预览裁切：截图后可以预览，也可以裁切后再发给 AI。
- Markdown 输出：AI 回复在程序内按 Markdown 渲染显示。
- 记录板：可以保存多条文本和图片记录，方便查看、复制和再次填入 AI 输入框。
- 内置练习记录：内置 13 条计算机网络练习题记录，便于复习和提问。
- 单文件打包：可以发布为一个自包含的 Windows exe 文件。

## AI 配置

打开程序后，进入 `AI问答` 标签页，展开 `AI设置`。

需要填写：

- `地址`：API 基础地址，例如 `http://example.com/v1`
- `密钥`：你的 API key

程序会在这个基础地址下调用 `/responses` 接口。默认模型为 `gpt-5.5-fast`，默认思考强度为 `xhigh`。

AI 配置会保存到本机用户目录：

```text
%AppData%\GhostPaste\ai-settings.json
```

源码中不包含默认调用地址，也不包含任何内置 API key。

## 截图策略

`AI问答` 页面提供多种截图方式：

- `全屏截图`：临时隐藏 GhostPaste 窗口后截取整个桌面。
- `目标窗口截图`：根据最近追踪到的目标窗口区域进行截图。
- `PrintWindow截图`：通过 Windows `PrintWindow` 请求目标窗口自行绘制图像，在部分被遮挡或非活动窗口场景下可能更有效。

如果遇到受保护内容、黑屏、空白或无法捕获的窗口，程序会给出失败提示，不会假装截图成功。

## 使用说明

1. 双击运行 `GhostPaste.exe`。
2. 如果要模拟粘贴，先点击目标软件里的输入框，再回到 GhostPaste，在 `粘贴发送` 中输入文本并点击 `发送`。
3. 可以用速度滑条调整发送速度，靠左更快，靠右更接近真人输入。
4. 如果要 AI 问答，进入 `AI问答`，先在 `AI设置` 中填写调用地址和密钥。
5. 输入问题后可以直接发送，也可以先截图，截图会自动附加到 AI 输入内容中。
6. 已附加的截图可以预览、裁切、清除，也可以保存到记录板。
7. 在 `记录板` 中可以添加文本和图片记录，之后可以复制或填回 AI 问答。
8. 发送文本或等待 AI 回复时，可以按 `Esc` 取消当前操作。

## 构建方法

项目基于 .NET 8 WPF，面向 Windows `win-x64`。

在项目根目录运行：

```powershell
dotnet restore
dotnet test .\GhostPaste.Tests\GhostPaste.Tests.csproj
dotnet publish .\GhostPaste.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o .\publish_out
```

构建完成后，单文件 exe 位于：

```text
publish_out\GhostPaste.exe
```

## 仓库说明

- 原始项目仓库：`dusk2999/GhostPaste`
- 当前独立实验仓库：`dusk2999/GhostPaste-ai-experiment`
- 本项目不会在源码中内置 AI 调用地址或密钥。
- 本地构建输出目录已通过 `.gitignore` 忽略。

## 开源协议

本项目采用 [GNU Affero General Public License v3.0 only](LICENSE)（`AGPL-3.0-only`）。
