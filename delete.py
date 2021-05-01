import os
import shutil

for parent, dirnames, filenames in os.walk('.'):
    for folder_name in dirnames:
        if folder_name.lower() == 'obj':
            folder_full_name = os.path.join(parent, folder_name)
            try:
                shutil.rmtree(folder_full_name)
            except:
                print(f"Could not delete {folder_full_name}")
    for fn in filenames:
        if fn.lower().endswith('.meta'):
            os.remove(os.path.join(parent, fn))