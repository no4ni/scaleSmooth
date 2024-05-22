# scaleSmooth
Algorithms for most-accurate upscaling image **without AI and neural network** (gray or color, smooth, rough, furry or contrastBold variants available)<br>
<div align="center"><a href="https://dzen.ru/suite/b70ea5e2-65bd-49ea-b0e4-49fc31e96df6">Мои эксперименты с изображениями</a><br>
<a href="https://dzen.ru/video/watch/6633aca1aef1ff543f59646e">Объяснение принипа работы и больше примеров и сравнений в FullHD</a>  
  <br><br>
<img src="https://raw.githubusercontent.com/no4ni/scaleSmooth/main/img/demo.png"/><br><br>
<b>Free and Open Source Image Upscaler</b></div><br>
<b>scaleSmooth</b> lets you enlarge and enhance low-resolution images using only math.<br>
Enlarge images and get more quality without losing accuracy (except contrastBoldScale) and hallucinations. It's just math!<br><br>

**scaleSmooth** is a windows application on .NET 8.0 on Visual Studio C#, but we'll be glad, if you create your repository of scaleSmooth / scaleRough / scaleFurry / contrastBoldScale / scaleSeparate / scaleBilinearApproximation for other platforms and languages (write me and I'll attach link to your repository) or commit here updates for optimization, batch, interface or more accuracy.<br>

**scaleSmooth** is algorithms by sequential calculations, so it's might be very slow (use fast/accurate regulator if it will be needed) and it can't parallelize by many CPU's or GPU (except scaleSeparate and scaleBilinearApproximation - now realized parallelize only by CPU's), but you can run as many copies of other methods at the same time as many logical CPU's you have without losing speed.<br>

Threshold, autoThreshold, Mean Cuvatute Blur, Median Blur, adjustment by Lanczos and other tools, which can help you get more usefull results for your specific needs are NOT included!
<table align="center"><tr><td>
<img src="https://raw.githubusercontent.com/no4ni/scaleSmooth/main/img/demo2.png"/></td><td>

# 🏃 Run
(required Windows 10+ x64, .NET 8.0+) 
- Download <a href="https://github.com/no4ni/scaleSmooth/raw/main/run/scaleSmooth-windows10-x64.zip">release</a>
- Unpack into any folder
- Run .exe<br></td></tr></table>

  <img src="https://raw.githubusercontent.com/no4ni/scaleSmooth/main/img/demo5.png"/>

# 🛠 Using in your projects
- Just copy necessary function (**ScaleSmoothGray** / **ScaleSmoothColor** / **ScaleRoughGray** and S255 / **ScaleRoughColor** and S255 / **ScaleFurryGray** and S255 / **ScaleFurryColor** and S255 / **ContrastBoldScaleGray**, S255f and S255 / **ContrastBoldScaleColor**, S255f and S255 / **ScaleSeparateGray**, Quadrilateral and Bilinear / **ScaleSeparateColor**, Quadrilateral and Bilinear / **ScaleBilinearApproximationGray**, Dist4 and Bilinear / **ScaleBilinearApproximationColor**, Dist4 and Bilinear) (you can harmless remove ProgressText from code)
- Call it with parameters (**image** as type Image, int **scale**, int **accuracy** - where 0 is fast, 100 is accurate)
- It returns new image as type **Image** <br>

<img src="https://raw.githubusercontent.com/no4ni/scaleSmooth/main/img/demo3.png"/>












