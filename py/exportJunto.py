import pyodbc
import xml.etree.ElementTree as ET
import xml.dom.minidom

# Conectarse a la base de datos SQL Server
conn = pyodbc.connect('DRIVER={SQL Server};SERVER=hotelsol.cnu4yaoycjpi.us-east-1.rds.amazonaws.com;DATABASE=hotelsol;UID=admin;PWD=hotelsol;Connect Timeout=30;Encrypt=True;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False')

# Crear un cursor para ejecutar consultas SQL
cursor = conn.cursor()

# Crear un elemento raíz para el árbol XML
root = ET.Element("hotelSol")

# Definir las tablas y sus consultas SQL
tablas = {
    "habitaciones": "SELECT numero, estado, tipohabitacionid FROM habitacion",
    "clientes": "SELECT dni, nombre, apellido1, apellido2, telefono, direccion, email, vip, estado FROM cliente",
    "reservas": "SELECT id, dni, habitacionid, tipoalojamientoid, tipotemporadaid, fechainicio, fechafin, factura, referido, checkin, cancelado, ofertaid FROM reserva"
}

# Iterar sobre las tablas y exportar los datos al XML
for tabla, consulta_sql in tablas.items():
    # Obtener el nombre de la tabla de la consulta
    nombre_tabla = consulta_sql.split("FROM")[1].split()[0]

    # Ejecutar la consulta SQL para seleccionar los datos que deseas exportar
    cursor.execute(consulta_sql)

    # Obtener los resultados de la consulta
    resultados = cursor.fetchall()

    # Crear un elemento para la tabla actual
    tabla_elemento = ET.SubElement(root, tabla)

    # Obtener los nombres de las columnas
    nombres_columnas = [columna[0] for columna in cursor.description]

    # Iterar sobre los resultados y agregarlos al árbol XML
    for fila in resultados:
        elemento_fila = ET.SubElement(tabla_elemento, nombre_tabla)
        for nombre_columna, valor in zip(nombres_columnas, fila):
            columna = ET.SubElement(elemento_fila, nombre_columna)
            columna.text = str(valor)

# Crear el árbol XML
xml_tree = ET.ElementTree(root)

# Guardar el árbol XML en un archivo
xml_tree.write("hotelSol.xml", encoding="utf-8", xml_declaration=True)

# Leer el archivo XML
with open("hotelSol.xml", "r") as file:
    xml_content = file.read()

# Parsear el XML
xml_parsed = xml.dom.minidom.parseString(xml_content)

# Obtener la versión indentada del XML como una cadena
xml_indented = xml_parsed.toprettyxml(indent="    ")

# Escribir el XML indentado en el archivo
with open("hotelSol.xml", "w") as file:
    file.write(xml_indented)

# Cerrar la conexión a la base de datos
conn.close()
