# scaleSmooth

Algorithms for most-accurate upscaling images ****without AI and neural network****<br><br>
<img src="https://raw.githubusercontent.com/no4ni/scaleSmooth/main/examples/demoSmooth.png"/><br>
<a href="#russian">Русский язык</a>
(gray or color / smooth, sharpness, semi-sharpness, monochrome or bold / fast or accuracy variants available)<br>
<br>
<img src="https://raw.githubusercontent.com/no4ni/scaleSmooth/main/examples/demoContrastBold.png"/><br><br>
<b>Free and Open Source Image Upscaler</b><br><br>
<b>scaleSmooth</b> lets you enlarge and enhance low-resolution images (up to 234 megapixels, e.g. 20420x11486 for 16:9) using only math.<br>
Enlarge images and get more quality without losing accuracy (in some cases, reverse adjustment is required) and hallucinations. It's just math!<br><br>

<img src="https://raw.githubusercontent.com/no4ni/scaleSmooth/main/examples/demoRoughFurry.png"/>

**scaleSmooth** is a windows application on .NET 8.0 on Visual Studio C#, but we'll be glad, if you create your repository for other platforms and languages (write me and I'll attach link to your repository) or **commit** here updates for translations for other languages, optimization, batch, interface or more accuracy. If you want improve anything we'll be glad for **pull request**, if you disagree with something boldly **fork** to your own repository<br>

Threshold, autoThreshold, Mean Curvatute Blur, Median Blur, adjustment by Lanczos, Antiringing and other tools, which can help you get more usefull results for your specific needs are NOT included!
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
- Just copy necessary function and it's subfunctions (f.e. **ScaleSmoothGray** / **ScaleSmoothColor** / **ScaleRoughGray** and S255 / **ScaleRoughColor** and S255 / **ScaleFurryGray** and S255 / **ScaleFurryColor** and S255 / **ContrastBoldScaleGray** and S255f / **ContrastBoldScaleColor** and S255f / **BoldScaleGray** and S255f / **BoldScaleColor** and S255f / **ScaleSeparateGray**, Quadrilateral and Bilinear / **ScaleSeparateColor**, Quadrilateral and Bilinear / **ScaleBilinearApproximationGray[GPU/Auto]**, Dist4 and Bilinear / **ScaleBilinearApproximationColor[GPU/Auto]**, Dist4 and Bilinear) (for GPU version install and using ILGPU and ILGPU.Algorithms packet) (you can harmless remove ProgressText from code)
- Call it with parameters (**image** as type Image, int **scale**, int **accuracy** - where 0 is fast, 100 is accurate)
- It returns new image as type **Image** <br>

<img src="https://raw.githubusercontent.com/no4ni/scaleSmooth/main/examples/demoBold.png"/>

# ℹ️ Description of methods
- **scaleSmooth**<br>
Most accurate for scenes where objects are completely in the image, but little bit blurred (much less than any interpolation) and grid structure is still visible<br>
Very fast - Slow, and you can process multiple images at the same time without losing speed (just run function in other thread or run .exe one more time)<br><br>

- **scaleSmoothContinuous**<br>
Most accurate for scenes where objects extend beyond the boundaries of the image, but little bit blurred (much less than any interpolation) and grid structure is still visible<br>
Very fast - Slow, and you can process multiple images at the same time without losing speed (just run function in other thread or run .exe one more time)<br><br>

- **scaleSmoothContrast**<br>
Very smooth, after reverse adjustment - most accurate for any scenes, but very contrast and grid structure is still visible<br>
Slow?, but you can process multiple images at the same time without losing speed (just run function in other thread or run .exe one more time)<br><br>

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
Gives beatiful, but almost monochrome result and there are Gibbs ringing artifacts (to avoid you can try several times x2-x4)<br>
Very very fast - fast, but you can't process multiple images at the same time without losing speed<br><br>

- **scaleBilinearApproximation**<br>
A clearly defined grid structure and may be present Gibbs ringing artifacts, but it's better than nearest neighbour<br>
Very very fast - Very slow, and you can't process multiple images at the same time without losing speed<br><br>

- **scaleBAContrast**<br>
Defined grid structure, little contrasty and may be present Gibbs ringing artifacts<br>
Very very fast - Very slow, and you can't process multiple images at the same time without losing speed<br><br>

- **scaleBASmoothContrast**<br>
Сontrasty and may be present grid structure<br>
Very very fast - Very slow, and you can't process multiple images at the same time without losing speed<br><br>

- **scaleBAmonochrome**<br>
Smooth, curve and same time sharpness result, but monochrome (colors may be distorted)<br>
Very slow, and you can't process multiple images at the same time without losing speed<br><br>

- **scaleBAmonochrome2**<br>
Sharpness edges, but colors may be distorted<br>
Very slow, and you can't process multiple images at the same time without losing speed<br><br>

- **scaleBAExtremum**<br>
Sharpen edges, but contrasty (for accuracy required reverse adjustment) and many Gibbs ringing artifacts<br>
Very very fast - Very slow, and you can't process multiple images at the same time without losing speed<br><br>

You can increase speed instead of accuracy and vice versa<br><br>



<img src="https://raw.githubusercontent.com/no4ni/scaleSmooth/main/examples/demoSeparateApproximation.png"/>
<br id="russian"></br>
<b>Алгоритмы для максимально точного масштабирования изображений без ИИ и нейронных сетей</b><br>
Доступно множество различных вариантов (серый или цветной / гладкий, резкий, полурезкий, монохромный или жирный / быстрый или точный)<br><br>
<a href="https://dzen.ru/suite/b70ea5e2-65bd-49ea-b0e4-49fc31e96df6">Мои эксперименты с изображениями</a><br>
Объяснение принипа работы, визуализация и больше примеров и сравнений в видео FullHD: <a href="https://dzen.ru/video/watch/6633aca1aef1ff543f59646e">#1</a>, <a href="https://dzen.ru/video/watch/66655d8129a5762762127928">#2</a><br>
Теория и объяснение в тексте и картинках: <a href="https://habr.com/ru/articles/812619/">#1</a>, <a href="https://habr.com/ru/articles/821309/">#2</a><br><br>
Бесплатный и открытый исходный код для масштабирования изображений<br><br>
scaleSmooth позволяет увеличивать и улучшать изображения с низким разрешением вплоть до 234 мегапикселей (например, 20420х11486 для 16:9), используя только математику.<br>
Увеличивайте изображения и получайте больше качества без потери точности (в некоторых случаях требуется обратная <a href="https://dzen.ru/video/watch/6633aca1aef1ff543f59646e">корректировка</a>) и галлюцинаций. Это просто математика!<br><br>
scaleSmooth это приложение Windows на .NET 8.0 на Visual Studio C#, но мы будем рады, если вы создадите свой репозиторий для других платформ и языков (напишите мне, и я прикреплю ссылку на ваш репозиторий) или закоммитите здесь обновления для переводов на другие языки, оптимизации, пакетной обработки, интерфейса или большей точности. Если вы хотите что-то улучшить, мы будем рады pull request, а если вы с чем-то не согласны, смело делайте fork в свой собственный репозиторий<br><br>

Порог, Авто-порог, Размытие по средней кривизне, Медианный фильтр, Корректировка Ланцошем, <a href="https://dzen.ru/video/watch/66655d8129a5762762127928">Антизвон</a> и другие инструменты, которые могут помочь вам получить более полезные результаты для ваших конкретных нужд, НЕ включены!<br><br>

🏃 Запустить:<br>
(требуется Windows 7?-10.0.19041+ x64, .NET 8.0+)<br>
- Загрузить <a href="https://github.com/no4ni/scaleSmooth/raw/main/run/scaleSmooth-WinX64.zip">релиз</a><br>
- Извлечь<br>
- Запустить .exe<br><br>

🛠 Использование в ваших проектах<br>
1. Просто скопируйте необходимую функцию и ее подфункции (например, **ScaleSmoothGray** / **ScaleSmoothColor** / **ScaleRoughGray** и S255 / **ScaleRoughColor** и S255 / **ScaleFurryGray** и S255 / **ScaleFurryColor** и S255 / **ContrastBoldScaleGray** и S255f / **ContrastBoldScaleColor** и S255f / **BoldScaleGray** и S255f / **BoldScaleColor** и S255f / **ScaleSeparateGray**, Quadrilateral и Bilinear / **ScaleSeparateColor**, Quadrilateral и Bilinear / **ScaleBilinearApproximationGray**[GPU/Auto], Dist4 и Bilinear / **ScaleBilinearApproximationColor**[GPU/Auto], Dist4 и Bilinear) (для использования версии для видеокарт установите nuget пакеты ILGPU и ILGPU.Algorithms) (вы можете безвредно удалить ProgressText из кода)<br>
2. Вызовите функцию с параметрами (Image изображение, int масштаб, int точность - где 0 - быстро, 100 - точно)<br>
3. Функция вернёт Вам новое изображение (с повышенным разрешением) как тип Image<br><br>

ℹ️ Описание методов<br>
- **scaleSmooth**<br>
Наиболее точный метод для сцен, где объекты полностью находятся на изображении, но результат немного размыт (гораздо меньше, чем при любой интерполяции) и структура сетки все еще видна<br>
Очень быстро - Медленно, и вы можете обрабатывать несколько изображений одновременно, не теряя скорости (просто запустите функцию в другом потоке или запустите .exe еще раз)<br><br>

- **scaleSmoothContinuous**<br>
Наиболее точный метод для сцен, где объекты выходят за границы изображения, но результат немного размыт (гораздо меньше, чем при любой интерполяции) и структура сетки все еще видна<br>
Очень быстро - Медленно, и вы можете обрабатывать несколько изображений одновременно, не теряя скорости (просто запустите функцию в другом потоке или запустите .exe еще раз)<br><br>

- **scaleSmoothContrast**<br>
Очень плавно-гладко-размыто, но после обратной корректировки - наиболее точный для любых сцен, но очень контрастный и структура сетки кое-где все еще видна
Медленно?, но вы можете обрабатывать несколько изображений одновременно, не теряя скорости (просто запустите функцию в другом потоке или запустите .exe еще раз)<br><br>

- **boldScale**<br>
Сетчатая структура, немного шумно и контрастно (для точности желательно последующее обратная корректировка), и слишком мелкие детали могут быть потеряны<br>
Очень быстро - Очень очень медленно, но вы можете обрабатывать несколько изображений одновременно без потери скорости<br><br>

- **contrastBoldScale**<br>
Идеальный результат, но слишком контрастный (для точности требуется последующее обратное исправление), и слишком мелкие детали теряются<br>
Очень быстро - Очень очень медленно, но вы можете обрабатывать несколько изображений одновременно без потери скорости<br><br>

- **scaleFurry**<br>
Красивый и детализированный результат, но только если большая версия должна быть монохромным изображением (только чисто черно-белым, или для цветного - только чисто красным, черным, зеленым, желтым, фуксией, синим, голубым и белым)<br>
Очень медленно - Очень очень медленно, но Вы можете обрабатывать несколько изображений одновременно без потери скорости<br><br>

- **scaleRough**<br>
Типографская растровая стилизация, но если большая версия должна быть монохромным изображением, то дает приемлемый результат для масштабирования<br>
Очень медленно, но вы можете обрабатывать несколько изображений одновременно без потери скорость<br><br>

- **scaleSeparate**<br>
Дает красивый, но почти монохромный результат и есть артефакты звона (чтобы избежать, можно попробовать несколько x2-x4)<br>
Очень очень быстро - быстро, но Вы не можете обрабатывать несколько изображений одновременно без потери скорости<br><br>

- **scaleBilinearApproximation**<br>
Четко определенная структура сетки и могут присутствовать артефакты звона Гиббса, но это лучше, чем ближайшим соседом<br>
Очень очень быстро - Очень медленно, и Вы не можете обрабатывать несколько изображений одновременно без потери скорости<br><br>

- **scaleBAContrast**<br>
Видна сеточная структура, немного контрастно и могут присутствовать артефакты звона<br>
Очень очень быстро - Очень медленно, и Вы не можете обрабатывать несколько изображений одновременно без потери скорости<br><br>

- **scaleBASmoothContrast**<br>
Контрастно и может быть видна сеточная структура<br>
Очень очень быстро - Очень медленно, и Вы не можете обрабатывать несколько изображений одновременно без потери скорости<br><br>

- **scaleBAmonochrome**<br>
Гладкий, криволинейный и в то же время чёткий результат, но монохромный (вследствие чего цвета могут искажаться)<br>
Очень медленно, и Вы не можете обрабатывать несколько изображений одновременно<br><br>

- **scaleBAmonochrome2**<br>
Чёткие границы, но цвета могут искажаться<br>
Очень медленно, и Вы не можете обрабатывать несколько изображений одновременно<br><br>

- **scaleBAExtremum**<br>
Чёткие края, но контрастно (для точности необходима обратная корректировка) и присутствует большой звон<br>
Очень очень быстро - Очень медленно, и Вы не можете обрабатывать несколько изображений одновременно<br><br>

Можно увеличить скорость за счёт точности и наоборот
