import pyodbc
import xml.etree.ElementTree as ET
import xml.dom.minidom
import argparse

# Conectarse a la base de datos SQL Server
conn = pyodbc.connect('DRIVER={SQL Server};SERVER=hotelsol.cnu4yaoycjpi.us-east-1.rds.amazonaws.com;DATABASE=hotelsol;UID=admin;PWD=hotelsol;Connect Timeout=30;Encrypt=True;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False')

# Crear un cursor para ejecutar consultas SQL
cursor = conn.cursor()

parser = argparse.ArgumentParser()
parser.add_argument("tablaExportada", help="Tabla exportada")
args = parser.parse_args()

# Definir las tablas y sus consultas SQL
tablas = {
    "clientes": "SELECT dni, nombre, apellido1, apellido2, telefono, direccion, email, vip, estado FROM cliente",
    "disponibilidades": "SELECT id, habitacion, fecha FROM disponibilidad",
    "habitaciones": "SELECT numero, estado, tipohabitacionid FROM habitacion",
    "ofertas": "SELECT id, descripcion, coeficiente FROM oferta",
    "reservas": "SELECT id, dni, habitacionid, tipoalojamientoid, tipotemporadaid, fechainicio, fechafin, factura, referido, checkin, cancelado, ofertaid FROM reserva",
    "reservasServicios": "SELECT id, reserva, servicio, fecha, cantidad FROM reservaServicio",
    "servicios": "SELECT id, descripcion, precio FROM servicio",
    "tiposAlojamiento": "SELECT id, descripcion, precio FROM tipoAlojamiento",
    "tiposHabitacion": "SELECT id, descripcion, precio FROM tipoHabitacion",
    "tiposTemporada": "SELECT id, descripcion, coeficiente FROM tipoTemporada"
}

# Obtener los campos de la tabla especificada
def obtener_campos_para_tabla(tabla):
    if tabla in tablas:
        cursor.execute(tablas[tabla])
        return [columna[0] for columna in cursor.description]
    else:
        print("La tabla especificada no existe en el diccionario.")
        return None

# Obtener los campos de la tabla especificada en el argumento
campos = obtener_campos_para_tabla(args.tablaExportada)

# Iterar sobre las tablas y exportar los datos a XML
for tabla, consulta_sql in tablas.items():
    if tabla == args.tablaExportada:
        tabla_nombre = consulta_sql.split("FROM")[1].strip()
        # Ejecutar la consulta SQL
        cursor.execute(consulta_sql)
        
        # Obtener los datos de la consulta
        registros = cursor.fetchall()
        
        # Crear un nuevo árbol XML con el elemento raíz <odoo>
        odoo_tree = ET.Element('odoo')
        
        # Agregar el elemento <data> al árbol XML
        data_element = ET.SubElement(odoo_tree, 'data', {'noupdate': '0'})
        
        # Iterar sobre los registros y crear elementos XML para cada uno
        for registro in registros:
            # Crear un elemento <record> con el atributo id y model
            cliente_id = registro[0]
            record_element = ET.SubElement(data_element, 'record', {'id': str(cliente_id), 'model': f'x_{tabla_nombre}'})
            
            # Crear elementos <field> para cada campo del registro
            for campo, valor in zip(campos, registro):
                field_element = ET.SubElement(record_element, 'field', {'name': f'x_{campo}'})
                field_element.text = str(valor) if valor is not None else ''

        # Guardar el árbol XML en un archivo
        xml_string = xml.dom.minidom.parseString(ET.tostring(odoo_tree)).toprettyxml()
        with open(f'{tabla}.xml', 'w', encoding='utf-8') as xml_file:
            xml_file.write(xml_string)
