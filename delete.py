import os

for parent, dirnames, filenames in os.walk('.'):
    for fn in filenames:
        if fn.lower().endswith('.meta'):
            os.remove(os.path.join(parent, fn))