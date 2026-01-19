import os

def print_tree(startpath):
    # Folders to ignore so the output isn't huge
    ignore_dirs = {'.git', '.vs', '.idea', 'bin', 'obj', '.settings'}
    
    print(f"Scanning: {os.path.abspath(startpath)}\n")

    for root, dirs, files in os.walk(startpath):
        # Modify dirs in-place to prevent walking into ignored folders
        dirs[:] = [d for d in dirs if d not in ignore_dirs]
        
        level = root.replace(startpath, '').count(os.sep)
        indent = '    ' * level
        print(f'{indent}[{os.path.basename(root)}]')
        
        subindent = '    ' * (level + 1)
        for f in files:
            print(f'{subindent}{f}')

if __name__ == '__main__':
    # Runs in the current directory
    print_tree('.')
    input("\nPress Enter to close...")