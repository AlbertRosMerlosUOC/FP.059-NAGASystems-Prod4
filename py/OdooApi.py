import os
import xml.etree.ElementTree as ET
import xmlrpc.client

class OdooAPI:
    def __init__(self, url, db, username, password, xml_directory):
        self.url = url
        self.db = db
        self.username = username
        self.password = password
        self.xml_directory = xml_directory  # Directorio donde se guardan los XML
        self.uid = self.authenticate()

    def authenticate(self):
        common = xmlrpc.client.ServerProxy(f'{self.url}/xmlrpc/2/common')
        uid = common.authenticate(self.db, self.username, self.password, {})
        if not uid:
            raise Exception("Autenticación fallida")
        return uid

     def process_all_xml(self):
        for filename in os.listdir(self.xml_directory):
            if filename.endswith(".xml"):
                file_path = os.path.join(self.xml_directory, filename)
                xml_tree = self.load_xml(file_path)
                self.process_xml(xml_tree, 'res.partner')

    def load_xml(self, file_path):
        tree = ET.parse(file_path)
        return tree

    def process_xml(self, xml_tree, model):
        root = xml_tree.getroot()
        for element in root.findall(model):
            data = {child.tag: child.text for child in element}
            self.create_record(model, data)

    def create_record(self, model, data):
        models = xmlrpc.client.ServerProxy(f'{self.url}/xmlrpc/2/object')
        model_id = models.execute_kw(self.db, self.uid, self.password,
                                     model, 'create', [data])
        return model_id

    def update_record(self, model, record_id, data):
        models = xmlrpc.client.ServerProxy(f'{self.url}/xmlrpc/2/object')
        models.execute_kw(self.db, self.uid, self.password,
                          model, 'write', [[record_id], data])

    def read_data(self, model, fields, limit=10):
        models = xmlrpc.client.ServerProxy(f'{self.url}/xmlrpc/2/object')
        data = models.execute_kw(self.db, self.uid, self.password,
                                 model, 'search_read', [[]],
                                 {'fields': fields, 'limit': limit})
        return data

# Uso de la clase
if __name__ == "__main__":
    odoo_api = OdooAPI("https://hotelsol.odoo.com/", "hotelSol", "galmirallm@uoc.edu", "e3309d91d8119bc5830b610e4cc30cddab0b1b1e")
    clientes = odoo_api.read_data('res.partner', ['name', 'country_id', 'comment'])
    for cliente in clientes:
        print(cliente)