# gigeSimpleGrabber
GigE Simple grabber
____

### English
Simple tool fot getting image from GigE Cameras as Cognex or Basler in terminal.

To set up the camera use Pylon - https://www.baslerweb.com/en/products/software/basler-pylon-camera-software-suite/

Don't want to build project - https://github.com/yacubovvs/gigeSimpleGrabber/tree/master/bin/Release

Only for Windows 7 or above.

Usage:
```
SimpleGrab.exe [camera serial number] [patch to new png file]  
SimpleGrab.exe [BMP|PNG|JPG|RAW|TIFF] [camera serial number] [patch to new png file]  
```

Example:
```
SimpleGrab.exe 22846677 "C:\Users\v.yakubov\Documents\grabber\out.png"
SimpleGrab.exe BMP 22846677 "C:\Users\v.yakubov\Documents\grabber\out.bmp"
```

Feel free to use it for commercial purposes.
Based on Pylon examples
____
### Русский
Простая утилита для получения изображения с камер с интерфейсовм GigE (таких как Basler или Cognex) через командную строку.

Для настройки камер, используйте утилиту Pylon - https://www.baslerweb.com/ru/produkty/programmnoje-obespechenie/basler-pylon-camera-software-suite/

Уже собранная версия находится в папке bin/Release - https://github.com/yacubovvs/gigeSimpleGrabber/tree/master/bin/Release

Только для Windows 7 или выше

Использование:
```
SimpleGrab.exe [серийный номер камеры] [путь к файлу]  
SimpleGrab.exe [BMP|PNG|JPG|RAW|TIFF] [серийный номер камеры] [путь к файлу]   
```

Пример:
```
SimpleGrab.exe 22846677 "C:\Users\v.yakubov\Documents\grabber\out.png"
SimpleGrab.exe BMP 22846677 "C:\Users\v.yakubov\Documents\grabber\out.bmp"
```

Разрешено использование в коммерчиских целях.
Основано на примерах Pylon
____

