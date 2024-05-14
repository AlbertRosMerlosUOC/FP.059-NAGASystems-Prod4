import xmlrpc.client
import os
import xml.etree.ElementTree as ET

class OdooConnector:
    def __init__(self, url, db, username, password):
        self.url = url
        self.db = db
        self.username = username
        self.password = password
        self.common = None
        self.models = None
        self.uid = None
        self.connected = False
        self.disconnect_timer = None

    def connect(self):
        if not self.connected:
            self.common = xmlrpc.client.ServerProxy('{}/xmlrpc/2/common'.format(self.url))
            self.uid = self.common.authenticate(self.db, self.username, self.password, {})
            if self.uid:
                self.models = xmlrpc.client.ServerProxy('{}/xmlrpc/2/object'.format(self.url))
                self.connected = True
                print("Conexion establecida correctamente.")
            else:
                print("No se pudo autenticar.")
        else:
            print("Ya estas conectado.")

    def disconnect(self):
        if self.connected:
            print("Desconectando...")
            self.common = None
            self.models = None
            self.uid = None
            self.connected = False
            print("Desconectado.")
        else:
            print("No estas conectado.")

    def send_xml_files(self):
        files = [f for f in os.listdir('.') if os.path.isfile(f) and f.endswith('.xml')]
        if files:
            print("Enviando archivos XML a Odoo...")
            for file in files:
                self.import_from_xml(file)
        else:
            print("No se encontraron archivos XML en el directorio.")

    def import_from_xml(self, xml_file_path):
        tree = ET.parse(xml_file_path)
        root = tree.getroot()
        for record in root.findall('.//record'):
            model_name = record.attrib.get('model')
            vals = {}
            for field in record.findall('field'):
                field_name = field.attrib.get('name')
                field_value = field.text
                vals[field_name] = field_value
            try:
                pk_val = vals['x_dni']
                registro_existente = self.models.execute_kw(self.db, self.uid, self.password, model_name, 'search', [[('x_dni', '=', pk_val)]])
                if registro_existente:
                    self.models.execute_kw(self.db, self.uid, self.password, model_name, 'write', [registro_existente, vals])
                    print(f"Registro modificado en el modelo {model_name} con ID {registro_existente}")
                else:
                    created_id = self.models.execute_kw(self.db, self.uid, self.password, model_name, 'create', [vals])
                    print(f"Registro creado en el modelo {model_name} con ID {created_id}")
            except Exception as e:
                print(f"Error al crear el registro en el modelo {model_name}: {e}")
        os.remove(xml_file_path)


# Datos de conexión
url = "https://hotelsol.odoo.com/"
db = "hotelsol"
username = "galmirallm@uoc.edu"
password = "e3309d91d8119bc5830b610e4cc30cddab0b1b1e"

# Crear una instancia del conector
connector = OdooConnector(url, db, username, password)

# Conectar a Odoo
connector.connect()

# Enviar archivos XML a Odoo
connector.send_xml_files()

# Desconectar de Odoo
connector.disconnect()
