# MultithreadedGZip

## Для чего
Это демонстрационный проект, который даёт возможность ознакомиться с моим стилем программирования.

### Постановка задачи
Создать эффективный многопоточный компрессор/декомпрессор обеспечивающий обработку файлов размером больше объема оперативной памяти.

### Требования
- Консольное приложение .NET (C#)
- Базироваться на использовании класса System.IO.Compression.GZipStream
- Использовать базовые классы для организации многопоточности и синхронизации.
- Занимать разумное место в памяти.
- Максимально использовать мощности центрального процессора.
- Обработка всех исключительных ситуаций.
- При успешном завершении код возврата должен быть 0, в случае ошибок 1.
- Параметры должны передаваться программе через командную строку.

Имеется набор **unit-тестов** (XUnit) на все режимы работы программы.

**Формат запуска**: 
*GZipTest <режим> <исходный файл> <результирующий файл> <технология>*

где *<режим>* может быть
- compress - компрессия исходного файла
- decompress - декомпрессия исходного файла

*<технология>* может быть
- blocking_collection - решение задачи с блокирующей коллекцией
- monitor - решение задачи с помощью мониторов
- semaphore - решение задачи с помощью семафоров
- *<не указан>* - в этом случае решение задачи с блокирующей коллекцией

**Пример**: *GZipTest compress data\source.xml data\compressed.gz semaphore*