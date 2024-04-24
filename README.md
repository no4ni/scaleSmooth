# scaleSmooth
Algorithm for most-accurate upscaling image **without AI and neural network** (gray or color, smooth or rough variants available)<br>
<div align="center"><a href="https://dzen.ru/suite/b70ea5e2-65bd-49ea-b0e4-49fc31e96df6">Мои эксперименты с изображениями</a><br><br>
<img src="https://raw.githubusercontent.com/no4ni/scaleSmooth/main/img/demo.png"/><br><br>
<b>Free and Open Source Image Upscaler</b></div><br>
<b>scaleSmooth</b> lets you enlarge and enhance low-resolution images using only math.<br>
Enlarge images and get more quality without losing accuracy and hallucinations. It's just math!<br><br>

**scaleSmooth** is a windows application on .NET 8.0 on Visual Studio C#, but we'll be glad, if you create your repository of scaleSmooth/scaleRough for other platforms and languages (write me and I'll attach link to your repository) or commit here updates for optimization, batch, interface or more accuracy.<br>

**scaleSmooth** is algorithms by sequential calculations, so it's might be very slow (use fast/accurate if it will be needed) and it can't parallelize by many CPU's or GPU, but you can run as many copies as many logical CPU's you have without losing speed.<br>

Threshold, autoThreshold, Mean Cuvatute Blur, Median Blur and other tools, which can help you get more usefull results for your specific needs are not included!



# 🏃 Run
(Windows 10+ x64) 
- Download <a href="https://github.com/no4ni/scaleSmooth/raw/main/run/scaleSmooth-windows10-x64.zip">release</a>
- Unpack into any folder
- Run .exe

# 🛠 Using in your projects
- Just copy necessary function (scaleSmoothGray/scaleSmoothColor/scaleRoughGray/scaleRoughColor)
- Call it with parameters (image as type Image, int scaleX, int accuracy - where 0 is fast, 100 is accurate)
- It returns new image as type Image 














