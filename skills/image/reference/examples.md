# Image 技能使用示例

## 概述

本文档提供了 Image 技能的详细使用示例，包括各种图像处理操作的命令行示例和预期输出。这些示例将帮助您快速上手并充分利用 Image 技能的功能。

## 基本图像处理示例

### 1. 调整图像大小

**功能说明**：调整图像的宽度和高度到指定尺寸。

**命令格式**：
```bash
image_aot resize <输入文件> <输出文件> <尺寸>
```

**示例 1**：将图像调整为 800x600 像素

```bash
image_aot resize input.jpg output.jpg 800x600
```

**预期输出**：
```
调整图像大小: input.jpg -> output.jpg, 尺寸: 800x600
图像处理完成! 用时: 125.432 ms
处理状态: 成功
输入尺寸: 1920x1080
输出尺寸: 800x600
文件大小: 123456 bytes
```

**示例 2**：使用命令别名

```bash
image_aot r input.jpg output.jpg 1024x768
```

### 2. 转换图像格式

**功能说明**：将图像从一种格式转换为另一种格式。

**命令格式**：
```bash
image_aot convert <输入文件> <输出文件> <格式>
```

**示例 1**：将 JPEG 转换为 PNG

```bash
image_aot convert input.jpg output.png png
```

**预期输出**：
```
转换图像格式: input.jpg -> output.png, 格式: png
图像处理完成! 用时: 89.214 ms
处理状态: 成功
输入格式: jpg
输出格式: png
文件大小: 234567 bytes
```

**示例 2**：将 PNG 转换为 WebP

```bash
image_aot c input.png output.webp webp
```

### 3. 裁剪图像

**功能说明**：从图像中裁剪指定区域。

**命令格式**：
```bash
image_aot crop <输入文件> <输出文件> <x> <y> <宽度> <高度>
```

**示例**：从图像左上角裁剪 500x500 像素的区域

```bash
image_aot crop input.jpg output.jpg 100 100 500 500
```

**预期输出**：
```
裁剪图像: input.jpg -> output.jpg, 区域: 100,100,500x500
图像处理完成! 用时: 67.890 ms
处理状态: 成功
输出尺寸: 500x500
文件大小: 98765 bytes
```

### 4. 旋转图像

**功能说明**：按指定角度旋转图像。

**命令格式**：
```bash
image_aot rotate <输入文件> <输出文件> <角度>
```

**示例 1**：将图像旋转 90 度

```bash
image_aot rotate input.jpg output.jpg 90
```

**预期输出**：
```
旋转图像: input.jpg -> output.jpg, 角度: 90
图像处理完成! 用时: 54.321 ms
处理状态: 成功
文件大小: 187654 bytes
```

**示例 2**：将图像旋转 45 度

```bash
image_aot ro input.jpg output.jpg 45
```

### 5. 翻转图像

**功能说明**：水平或垂直翻转图像。

**命令格式**：
```bash
image_aot flip <输入文件> <输出文件> <方向>
```

**示例 1**：水平翻转图像

```bash
image_aot flip input.jpg output.jpg horizontal
```

**预期输出**：
```
翻转图像: input.jpg -> output.jpg, 方向: horizontal
图像处理完成! 用时: 34.567 ms
处理状态: 成功
文件大小: 176543 bytes
```

**示例 2**：垂直翻转图像

```bash
image_aot f input.jpg output.jpg vertical
```

### 6. 添加水印

**功能说明**：在图像上添加文本水印。

**命令格式**：
```bash
image_aot watermark <输入文件> <输出文件> <水印文本> <位置>
```

**示例**：在图像右下角添加版权水印

```bash
image_aot watermark input.jpg output.jpg "© 2024 版权所有" bottom-right
```

**预期输出**：
```
添加水印: input.jpg -> output.jpg, 文本: © 2024 版权所有, 位置: bottom-right
图像处理完成! 用时: 76.543 ms
处理状态: 成功
文件大小: 210987 bytes
```

### 7. 应用滤镜

**功能说明**：对图像应用各种滤镜效果。

**命令格式**：
```bash
image_aot filter <输入文件> <输出文件> <滤镜类型>
```

**示例 1**：应用灰度滤镜

```bash
image_aot filter input.jpg output.jpg grayscale
```

**预期输出**：
```
应用滤镜: input.jpg -> output.jpg, 滤镜: grayscale
图像处理完成! 用时: 43.210 ms
处理状态: 成功
文件大小: 154321 bytes
```

**示例 2**：应用模糊滤镜

```bash
image_aot fi input.jpg output.jpg blur
```

### 8. 获取图像元数据

**功能说明**：获取图像的详细信息，如格式、尺寸、文件大小等。

**命令格式**：
```bash
image_aot metadata <输入文件>
```

**示例**：

```bash
image_aot metadata input.jpg
```

**预期输出**：
```
获取图像元数据: input.jpg
元数据获取完成! 用时: 12.345 ms
处理状态: 成功
格式: jpg
尺寸: 1920x1080
文件大小: 2345678 bytes
创建时间: 2024-01-01 12:00:00
修改时间: 2024-01-01 12:30:00
```

### 9. 优化图像

**功能说明**：优化图像质量和大小，减少文件体积。

**命令格式**：
```bash
image_aot optimize <输入文件> <输出文件> [<质量>]
```

**示例**：优化图像，设置质量为 80

```bash
image_aot optimize input.jpg output.jpg 80
```

**预期输出**：
```
优化图像: input.jpg -> output.jpg, 质量: 80
图像处理完成! 用时: 98.765 ms
处理状态: 成功
原始大小: 2345678 bytes
优化大小: 1234567 bytes
压缩率: 47.36%
```

## 高级功能示例

### 10. 批量处理图像

**功能说明**：对多个图像进行批量操作，如调整大小、转换格式等。

**命令格式**：
```bash
image_aot batch <输入目录> <输出目录> <操作> [<参数>]
```

**示例 1**：批量调整图像大小

```bash
image_aot batch input_folder output_folder resize 800x600
```

**预期输出**：
```
批量处理: input_folder -> output_folder, 操作: resize
批量处理完成! 用时: 567.890 ms
处理状态: 成功
处理文件数: 10
成功: 10
失败: 0
```

**示例 2**：批量转换图像格式

```bash
image_aot b input_folder output_folder convert jpg
```

### 11. 运行基准测试

**功能说明**：运行性能基准测试，评估图像处理操作的性能。

**命令格式**：
```bash
image_aot benchmark [<操作>] [<迭代次数>]
```

**示例**：运行调整大小操作的基准测试，迭代 10 次

```bash
image_aot benchmark resize 10
```

**预期输出**：
```
运行基准测试: 操作=resize, 迭代次数=10
基准测试完成!
总用时: 1234.567 ms
成功: 10
失败: 0
平均每操作: 123.457 ms
每秒操作数: 8.10 ops/s
```

### 12. 显示配置信息

**功能说明**：显示当前的配置信息。

**命令格式**：
```bash
image_aot config
```

**示例**：

```bash
image_aot config
```

**预期输出**：
```
Image 配置:
============================================================
默认格式: png
默认质量: 85
启用缓存: True
缓存大小: 100
缓存过期: 00:30:00
启用并行处理: True
最大并行度: 8
临时目录: C:\Users\User\AppData\Local\Temp\
支持的格式: jpeg, jpg, png, gif, webp, bmp
```

### 13. 显示帮助信息

**功能说明**：显示命令帮助信息。

**命令格式**：
```bash
image_aot help
```

**示例**：

```bash
image_aot help
```

**预期输出**：
```
Image AOT 引擎 命令帮助:
============================================================
resize (r)      - 调整图像大小
convert (c)     - 转换图像格式
crop (cr)       - 裁剪图像
rotate (ro)     - 旋转图像
flip (f)        - 翻转图像
watermark (w)   - 添加水印
filter (fi)     - 应用滤镜
metadata (m)    - 获取图像元数据
optimize (o)    - 优化图像
batch (b)       - 批量处理图像
benchmark (bm)  - 运行基准测试
config (co)     - 显示配置信息
help (h, ?)     - 显示帮助信息
```

## 常见使用场景示例

### 场景 1：网站图像优化

**需求**：为网站准备图像，包括调整大小、转换格式和优化。

**解决方案**：

1. **调整图像大小**：
   ```bash
   image_aot batch input_images web_images resize 1200x800
   ```

2. **转换为 WebP 格式**：
   ```bash
   image_aot batch web_images webp_images convert webp
   ```

3. **优化图像**：
   ```bash
   image_aot batch webp_images optimized_images optimize 85
   ```

### 场景 2：社交媒体图像准备

**需求**：为不同社交媒体平台准备图像，调整为合适的尺寸。

**解决方案**：

1. **Facebook 封面**：
   ```bash
   image_aot resize input.jpg facebook_cover.jpg 1640x859
   ```

2. **Instagram 帖子**：
   ```bash
   image_aot resize input.jpg instagram_post.jpg 1080x1080
   ```

3. **Twitter 头部**：
   ```bash
   image_aot resize input.jpg twitter_header.jpg 1500x500
   ```

### 场景 3：批量添加水印

**需求**：为多个图像添加版权水印。

**解决方案**：

```bash
image_aot batch input_images watermarked_images watermark "© 2024 版权所有" bottom-right
```

### 场景 4：图像库整理

**需求**：整理图像库，统一图像格式和大小。

**解决方案**：

1. **批量转换为 JPEG**：
   ```bash
   image_aot batch photo_library jpeg_library convert jpg
   ```

2. **批量调整大小**：
   ```bash
   image_aot batch jpeg_library resized_library resize 1920x1080
   ```

3. **批量优化**：
   ```bash
   image_aot batch resized_library optimized_library optimize 85
   ```

## 性能测试示例

### 测试不同操作的性能

**示例**：测试各种图像处理操作的性能

```bash
# 测试调整大小
image_aot benchmark resize 20

# 测试格式转换
image_aot benchmark convert 20

# 测试滤镜应用
image_aot benchmark filter 20
```

**预期输出**：
```
# 调整大小测试结果
运行基准测试: 操作=resize, 迭代次数=20
基准测试完成!
总用时: 2456.789 ms
成功: 20
失败: 0
平均每操作: 122.839 ms
每秒操作数: 8.14 ops/s

# 格式转换测试结果
运行基准测试: 操作=convert, 迭代次数=20
基准测试完成!
总用时: 1890.123 ms
成功: 20
失败: 0
平均每操作: 94.506 ms
每秒操作数: 10.58 ops/s

# 滤镜应用测试结果
运行基准测试: 操作=filter, 迭代次数=20
基准测试完成!
总用时: 3210.456 ms
成功: 20
失败: 0
平均每操作: 160.523 ms
每秒操作数: 6.23 ops/s
```

### 测试并行处理性能

**示例**：测试并行处理与串行处理的性能差异

```bash
# 启用并行处理
set IMAGE_PARALLEL_ENABLED=true
image_aot batch large_folder output_folder resize 800x600

# 禁用并行处理
set IMAGE_PARALLEL_ENABLED=false
image_aot batch large_folder output_folder_serial resize 800x600
```

**预期输出**：
```
# 并行处理
批量处理: large_folder -> output_folder, 操作: resize
批量处理完成! 用时: 1234.567 ms
处理状态: 成功
处理文件数: 50
成功: 50
失败: 0

# 串行处理
批量处理: large_folder -> output_folder_serial, 操作: resize
批量处理完成! 用时: 4567.890 ms
处理状态: 成功
处理文件数: 50
成功: 50
失败: 0
```

## 故障排除示例

### 示例 1：文件不存在

**问题**：输入文件不存在

**命令**：
```bash
image_aot resize non_existent.jpg output.jpg 800x600
```

**预期错误**：
```
错误: 找不到文件 "non_existent.jpg"
```

**解决方案**：检查文件路径是否正确，确保文件存在。

### 示例 2：不支持的格式

**问题**：尝试处理不支持的图像格式

**命令**：
```bash
image_aot convert input.tiff output.jpg jpg
```

**预期错误**：
```
错误: 不支持的文件格式: tiff
```

**解决方案**：使用支持的图像格式，如 JPEG、PNG、GIF、WebP 或 BMP。

### 示例 3：参数错误

**问题**：提供了无效的参数

**命令**：
```bash
image_aot resize input.jpg output.jpg invalid_size
```

**预期错误**：
```
错误: 无效的尺寸格式，请使用 WxH 格式
```

**解决方案**：使用正确的参数格式，如 `800x600`。

## 总结

本文档提供了 Image 技能的详细使用示例，涵盖了所有主要功能。通过这些示例，您可以快速上手并充分利用 Image 技能的强大功能。

### 提示

- **使用命令别名**：对于常用命令，可以使用别名来减少输入，如 `r` 代替 `resize`。
- **批量处理**：对于多个图像的相同操作，使用批量处理命令可以大大提高效率。
- **并行处理**：对于大量图像处理，确保启用并行处理以提高速度。
- **图像格式**：根据具体使用场景选择合适的图像格式，如 WebP 格式通常具有更好的压缩率。
- **图像质量**：在图像质量和文件大小之间取得平衡，根据具体需求设置合适的质量值。

通过合理使用 Image 技能，您可以高效地处理各种图像处理任务，提高工作效率并获得高质量的结果。
