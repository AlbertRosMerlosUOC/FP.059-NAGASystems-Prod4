import xmlrpc.client

url = "https://hotelsol.odoo.com/"
db = "hotelsol"
username = "galmirallm@uoc.edu"
userpass = "e3309d91d8119bc5830b610e4cc30cddab0b1b1e"

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
        self.vals

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
                with open(file, 'rb') as xml_file:
                    xml_data = xml_file.read()
                    model_name = self.get_model_name_from_xml(xml_data)
                    if model_name:
                        self.models.create(self.db, self.uid, self.password, {'model_name': model_name, 'xml_data': xml_data})
                        print(f"Archivo {file} enviado correctamente al modelo {model_name}.")
                    else:
                        print(f"No se pudo obtener el nombre del modelo del archivo {file}.")
        else:
            print("No se encontraron archivos XML en el directorio.")

    def envio_prueba(self):
        vals = {
            'x_dni': "1223344",
            'x_name': "Prueba",
            'x_apellido1' : "Apellido1",
            'x_apellido2' : "Apellido2",
            'x_direccion' : "Direccion",
            'x_email' : "Email",
            'x_estado' : "1",
            'x_telefono' : "22222",
            'x_vip' : "0" 
        }
        created_id = self.models.execute_kw(db, self.uid, self.password, 'x_cliente', 'create', [vals])
        print("created record ->", created_id)

    def get_model_name_from_xml(self, xml_data):
        try:
            root = ET.fromstring(xml_data)
            record_element = root.find('.//record')
            model_name = record_element.attrib.get('model', '')
            return model_name
        except Exception as e:
            print(f"Error al obtener el nombre del modelo del archivo XML: {e}")
            return None


# Crear una instancia del conector
connector = OdooConnector(url, db, username, userpass)

# Conectar a Odoo
connector.connect()

# Enviar archivos XML a Odoo
#connector.send_xml_files()
connector.envio_prueba()

# Desconectar de Odoo
connector.disconnect()
