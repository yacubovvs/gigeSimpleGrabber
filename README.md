# gigeSimpleGrabber
GigE Simple grabber
____

### English
Simple tool fot getting image from GigE Cameras as Cognex or Basler in terminal.

To set up the camera use Pylon - https://www.baslerweb.com/en/products/software/basler-pylon-camera-software-suite/
Required Pylon 6+ util with developer package and Net FrameWork 2.0

Don't want to build project - https://github.com/yacubovvs/gigeSimpleGrabber/tree/master/bin/Release

Only for Windows 7 or above.

Usage:
```
Parameters usage
    -s  Camera serial number
    -o  Path to file
    -d  Inter package delay in ticks (default 1000)
    -a  Attempts tо grab image (default 1)
    -p  Package size (default 1500)
    -e  Exposure time (default 35000)
    -f  Image format [BMP|PNG|JPG|RAW|TIFF]
```

Example:
```
SimpleGrab.exe -s 23348170 -o "C:\LayerAggregation\gigE_Grabber\temp\test.png" -f BMP -p 9000 -d 5000 -a 10 -e 35000
```

Feel free to use it for commercial purposes.
Based on Pylon examples.

Project for Visual Studio 2019. Was tested in Windows 7 and Windows 10
____
### Русский
Простая утилита для получения изображения с камер с интерфейсовм GigE (таких как Basler или Cognex) через командную строку.

Для настройки камер, используйте утилиту Pylon - https://www.baslerweb.com/ru/produkty/programmnoje-obespechenie/basler-pylon-camera-software-suite/
Для запуска приложения необхоима установленная устилиа Pylon 6+ с пакетом разработчика и Net FrameWork 2.0

Уже собранная версия находится в папке bin/Release - https://github.com/yacubovvs/gigeSimpleGrabber/tree/master/bin/Release

Только для Windows 7 или выше

Использование:
```
Параметры
    -s  Серийный номер камеры
    -o  Путь к изображению
    -d  Время задержки между пакетами (по умолчанию 1000)
    -a  Количество попыток получения изображения (по умолчанию 1)
    -p  Размер пакета (по умолчанию 1500)
    -e  Время экспозиции (по умолчанию 35000)
    -f  Формат изображения [BMP|PNG|JPG|RAW|TIFF]
```

Пример:
```
SimpleGrab.exe -s 23348170 -o "C:\LayerAggregation\gigE_Grabber\temp\test.png" -f BMP -p 9000 -d 5000 -a 10 -e 35000
```

Разрешено использование в коммерчиских целях.
Основано на примерах Pylon.

Проект для Visual Studio 2019. Был протестирован в Windows 7 и Windows 10
____

