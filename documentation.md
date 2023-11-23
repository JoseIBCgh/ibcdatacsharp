# MainWindow
Contiene todos los callbacks de la api
# VirtualToolBar
Clase que usan tanto la toolBar como la MenuBar (ya que ambas comparten bastantes botones). Sirve para ejecutar las acciones (record, capture).
### VirtualToolBarProperties
Esto son como Commands sirven para establecer las reglas de activación y desactivación de los botones de la VirtualToolBar
# Graphs
## Models
Contiene todos los modelos de scottplot.
## OneIMU, TwoIMU y Sagital
Contienen las vistas de los graficos
## GraphData
Esto es para cargar los csv y generar la estructura de datos para stremear en replay.
## GraphInterface.cs
Interfaz que comparten todas las vistas de grafos.
## GraphManager.cs
Maneja tanto el streaming, grabación y replay. Tambien hace los calculos de streaming de 1 y 2 IMUs.
# Filters
Aqui estan las funciones para aplicar los filtros
## Filter.cs
Clase base de todos los filtros
## FilterManager.cs
Se encarga de manejar los filtros
### None.cs
Sin filtro
### EKF.cs, Mahoney.cs, Madgwick.cs
Clases concretas de cada filtro
# FileSaver
Todo lo encargado de hacer el record
## FileSaver.cs
Se encarga de grabar. Guarda los buffers para el video y csv. Tiene funciones para añadir lineas/frames a los bufers
## SaveArgs.cs
Argumentos del evento que se triggea cuando se pulsa a grabar. Determina si hay que grabar video, csv y el directorio.
## VideoBuffer.cs
Esto fue un experimento. No se usa.
# FileBrowser
Esto lo copie de un code project
# DeviceList
Lista de dispositivos
## TreeClasses
Diferentes objetos de la lista
## Converters
### BatteryConverter
Para añadir el '%' a la bateria
### ConnectedConverter
Para dibujar el circulo rojo o verde
### JAEnabledConverter
