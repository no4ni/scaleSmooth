# scaleSmooth
Algorithms for most-accurate upscaling image **without AI and neural network**<br><br>
<img src="https://raw.githubusercontent.com/no4ni/scaleSmooth/main/examples/demoSmooth.png"/><br>
(gray or color/ smooth, rough, furry, separate, approximation, bold or contrastBold / fast or accuracy variants available)<br>
<div align="center"><a href="https://dzen.ru/suite/b70ea5e2-65bd-49ea-b0e4-49fc31e96df6">Мои эксперименты с изображениями</a><br>
Объяснение принипа работы и больше примеров и сравнений в FullHD: <a href="https://dzen.ru/video/watch/6633aca1aef1ff543f59646e">#1</a>, <a href="https://dzen.ru/video/watch/66655d8129a5762762127928">#2</a>  
  <br><br>
<img src="https://raw.githubusercontent.com/no4ni/scaleSmooth/main/examples/demoContrastBold.png"/><br><br>
<b>Free and Open Source Image Upscaler</b></div><br>
<b>scaleSmooth</b> lets you enlarge and enhance low-resolution images using only math.<br>
Enlarge images and get more quality without losing accuracy (except contrastBoldScale) and hallucinations. It's just math!<br><br>

<img src="https://raw.githubusercontent.com/no4ni/scaleSmooth/main/examples/demoRoughFurry.png"/>

**scaleSmooth** is a windows application on .NET 8.0 on Visual Studio C#, but we'll be glad, if you create your repository of scaleSmooth / scaleRough / scaleFurry / contrastBoldScale / scaleSeparate / scaleBilinearApproximation for other platforms and languages (write me and I'll attach link to your repository) or **commit** here updates for translations for other languages, optimization, batch, interface or more accuracy. If you want improve anything we'll be glad for **pull request**, if you disagree with something boldly **fork** to your own repository<br>

Threshold, autoThreshold, Mean Cuvatute Blur, Median Blur, adjustment by Lanczos, Antiringing and other tools, which can help you get more usefull results for your specific needs are NOT included!
<table align="center"><tr><td width="50%">
<img src="https://raw.githubusercontent.com/no4ni/scaleSmooth/main/examples/demoText.png"/></td><td>

# 🏃 Run
(required Windows 7?-10.0.19041+ x64, .NET 8.0+) 
- Download <a href="https://github.com/no4ni/scaleSmooth/raw/main/run/scaleSmooth-WinX64.zip">release</a>
- Extract
- Run .exe<br>
<img src="https://raw.githubusercontent.com/no4ni/scaleSmooth/main/examples/demoBilinear.png"/>
</td></tr></table>

  <img src="https://raw.githubusercontent.com/no4ni/scaleSmooth/main/examples/demoSmoothRough.png"/>

# 🛠 Using in your projects
- Just copy necessary function (**ScaleSmoothGray** / **ScaleSmoothColor** / **ScaleRoughGray** and S255 / **ScaleRoughColor** and S255 / **ScaleFurryGray** and S255 / **ScaleFurryColor** and S255 / **ContrastBoldScaleGray** and S255f / **ContrastBoldScaleColor** and S255f / **BoldScaleGray** and S255f / **BoldScaleColor** and S255f / **ScaleSeparateGray**, Quadrilateral and Bilinear / **ScaleSeparateColor**, Quadrilateral and Bilinear / **ScaleBilinearApproximationGray[GPU/Auto]**, Dist4 and Bilinear / **ScaleBilinearApproximationColor[GPU/Auto]**, Dist4 and Bilinear) (you can harmless remove ProgressText from code)
- Call it with parameters (**image** as type Image, int **scale** (2-255), int **accuracy** - where 0 is fast, 100 is accurate)
- It returns new image as type **Image** <br>

<img src="https://raw.githubusercontent.com/no4ni/scaleSmooth/main/examples/demoBold.png"/>

# ℹ️ Description of methods
- **scaleSmooth**<br>
Most accurate for scenes where objects are completely in the image, but little bit blurred (much less than any interpolation) and grid structure is still visible<br>
Very fast - Slow, and you can process multiple images at the same time without losing speed (just run function in other thread or run .exe one more time)<br><br>

- **scaleSmoothContinuous**<br>
Most accurate for scenes where objects extend beyond the boundaries of the image, but little bit blurred (much less than any interpolation) and grid structure is still visible<br>
Very fast - Slow, and you can process multiple images at the same time without losing speed (just run function in other thread or run .exe one more time)<br><br>

- **boldScale**<br>
Grid structure, little bit noisy and contrasty (for accuracy, subsequent reverse correction is desirable) and too small details may lost<br>
Very fast - Very very slow, but you can process multiple images at the same time without losing speed<br><br>

- **contrastBoldScale**<br>
Perfect result, but too contrasty (for accuracy, subsequent reverse correction is required) and too small details are lost<br>
Very fast - Very very slow, but you can process multiple images at the same time without losing speed<br><br>

- **scaleFurry**<br>
Beautiful and detailed result, but only if bigger version must be monochrome image (only pure black and white, or for color - only pure red, black, green, yellow, fuchsia, blue, cyan and white)<br>
Very slow - Very very slow, but you can process multiple images at the same time without losing speed<br><br>

- **scaleRough**<br>
Typographic raster stylization, but if bigger version must be monochrome image it gives acceptable upscaling<br>
Very slow, but you can process multiple images at the same time without losing speed<br><br>

- **scaleSeparate**<br>
Gives beatiful, but almost monochrome result and there are Gibbs ringing artifacts<br>
Very very fast - fast, but you can't process multiple images at the same time without losing speed<br><br>

- **scaleBilinearApproximation**<br>
A clearly defined grid structure and Gibbs ringing artifacts are present, but even if these shortcomings are not removed with other tools, it is more accurate than Lanczos and clearer than Lanczos and Bicubic<br>
Very very fast - Very slow, and you can't process multiple images at the same time without losing speed<br><br>

You can increase speed instead of accuracy and vice versa<br><br>

<img src="https://raw.githubusercontent.com/no4ni/scaleSmooth/main/examples/demoSeparateApproximation.png"/>








