#!/bin/bash
set -e

# Delete the empty NotificationApp folders if they exist
rm -rf NotificationApp.*
rm -f NotificationApp.sln

# Create projects
dotnet new sln -n NotificationApp
dotnet new classlib -n NotificationApp.Models
dotnet new classlib -n NotificationApp.DataAccess
dotnet new classlib -n NotificationApp.Business
dotnet new console -n NotificationApp.Presentation

# Add to sln
dotnet sln add NotificationApp.Models NotificationApp.DataAccess NotificationApp.Business NotificationApp.Presentation

# Add references
dotnet add NotificationApp.DataAccess reference NotificationApp.Models
dotnet add NotificationApp.Business reference NotificationApp.Models NotificationApp.DataAccess
dotnet add NotificationApp.Presentation reference NotificationApp.Models NotificationApp.DataAccess NotificationApp.Business

# Add packages
dotnet add NotificationApp.DataAccess package Microsoft.EntityFrameworkCore --version 8.0.0
dotnet add NotificationApp.DataAccess package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.0
dotnet add NotificationApp.Presentation package Microsoft.EntityFrameworkCore.Design --version 8.0.0

# Move files
mv Models/* NotificationApp.Models/
mv Interfaces/* NotificationApp.Models/
mv DataAccessLayer/* NotificationApp.DataAccess/
mv BusinessLayer/* NotificationApp.Business/
mkdir -p NotificationApp.Business/NotificationSenders
mv NotificationSenders.cs/* NotificationApp.Business/NotificationSenders/
mv Program.cs NotificationApp.Presentation/

# Cleanup old
rm -rf Models Interfaces DataAccessLayer BusinessLayer NotificationSenders.cs Notification_CRUD.csproj bin obj

# Move ValidationHelper
mv NotificationApp.Business/ValidationHelper.cs NotificationApp.Models/

# Delete Class1.cs
rm NotificationApp.Models/Class1.cs NotificationApp.DataAccess/Class1.cs NotificationApp.Business/Class1.cs

# Fix namespaces script
cat << 'EOF' > fix_namespaces.py
import os

replacements = {
    "namespace Notification_CRUD.Models": "namespace NotificationApp.Models",
    "namespace Notification_CRUD.DataAccessLayer": "namespace NotificationApp.DataAccess",
    "namespace Notification_CRUD.BusinessLayer": "namespace NotificationApp.Business",
    "namespace Notification_CRUD.Interfaces": "namespace NotificationApp.Models",
    "namespace Notification_CRUD.NotificationSenders": "namespace NotificationApp.Business.NotificationSenders",
    "namespace Notification_CRUD": "namespace NotificationApp",
    "using Notification_CRUD.Models;": "using NotificationApp.Models;",
    "using Notification_CRUD.DataAccessLayer;": "using NotificationApp.DataAccess;",
    "using Notification_CRUD.BusinessLayer;": "using NotificationApp.Business;",
    "using Notification_CRUD.Interfaces;": "using NotificationApp.Models;",
    "using Notification_CRUD.NotificationSenders;": "using NotificationApp.Business.NotificationSenders;",
    
    "namespace NotificationSystem.Models": "namespace NotificationApp.Models",
    "namespace NotificationSystem.DataAccessLayer": "namespace NotificationApp.DataAccess",
    "namespace NotificationSystem.BusinessLayer.NotificationSenders": "namespace NotificationApp.Business.NotificationSenders",
    "namespace NotificationSystem.BusinessLayer": "namespace NotificationApp.Business",
    "namespace NotificationSystem.Interfaces": "namespace NotificationApp.Models",
    "namespace NotificationSystem": "namespace NotificationApp",
    "using NotificationSystem.Models;": "using NotificationApp.Models;",
    "using NotificationSystem.DataAccessLayer;": "using NotificationApp.DataAccess;",
    "using NotificationSystem.BusinessLayer;": "using NotificationApp.Business;",
    "using NotificationSystem.Interfaces;": "using NotificationApp.Models;",
    "using NotificationSystem.BusinessLayer.NotificationSenders;": "using NotificationApp.Business.NotificationSenders;"
}

for root, dirs, files in os.walk('.'):
    if 'obj' in root or 'bin' in root or '.git' in root:
        continue
    for file in files:
        if file.endswith('.cs'):
            filepath = os.path.join(root, file)
            with open(filepath, 'r') as f:
                content = f.read()
            original = content
            for k, v in replacements.items():
                content = content.replace(k, v)
            if original != content:
                with open(filepath, 'w') as f:
                    f.write(content)
EOF

python3 fix_namespaces.py
rm fix_namespaces.py

# Fix ValidationHelper namespace
sed -i '' 's/namespace NotificationApp.Business/namespace NotificationApp.Models/' NotificationApp.Models/ValidationHelper.cs

# Fix User.cs
sed -i '' '/using NotificationApp.Business;/d' NotificationApp.Models/User.cs

# Fix EmailNotification.cs and SmsNotification.cs namespace
sed -i '' 's/namespace NotificationApp.Models.NotificationSenders/namespace NotificationApp.Business.NotificationSenders/' NotificationApp.Business/NotificationSenders/EmailNotification.cs
sed -i '' 's/namespace NotificationApp.NotificationSenders/namespace NotificationApp.Business.NotificationSenders/' NotificationApp.Business/NotificationSenders/EmailNotification.cs
sed -i '' 's/namespace NotificationApp.Models.NotificationSenders/namespace NotificationApp.Business.NotificationSenders/' NotificationApp.Business/NotificationSenders/SmsNotification.cs
sed -i '' 's/namespace NotificationApp.NotificationSenders/namespace NotificationApp.Business.NotificationSenders/' NotificationApp.Business/NotificationSenders/SmsNotification.cs

# Fix Program.cs
sed -i '' '/using NotificationApp.Presentation;/d' NotificationApp.Presentation/Program.cs
sed -i '' 's/partial class Program/    partial class Program/' NotificationApp.Presentation/Program.cs
sed -i '' 's/static void Main/        static void Main/' NotificationApp.Presentation/Program.cs
sed -i '' 's/var consoleMenu/            var consoleMenu/' NotificationApp.Presentation/Program.cs
sed -i '' 's/consoleMenu.Start()/            consoleMenu.Start()/' NotificationApp.Presentation/Program.cs
# Adjust the curly braces
echo "}" >> NotificationApp.Presentation/Program.cs
# Actually I'll just use a python script to wrap Program in namespace correctly instead of sed to be safer.

cat << 'EOF' > fix_program.py
with open("NotificationApp.Presentation/Program.cs", "r") as f:
    lines = f.readlines()
    
# Find the start of partial class Program
idx = 0
for i, line in enumerate(lines):
    if "partial class Program" in line:
        idx = i
        break
        
lines[idx-1] = "    partial class Program\n"
lines[idx] = "    {\n"
lines[idx+1] = "        static void Main(string[] args)\n"
lines[idx+2] = "        {\n"
lines[idx+3] = "            var consoleMenu = new ConsoleMenu();\n"
lines[idx+4] = "            consoleMenu.Start();\n"
lines[idx+5] = "        }\n"
lines[idx+6] = "    }\n"
lines[idx+7] = "}\n"

with open("NotificationApp.Presentation/Program.cs", "w") as f:
    f.writelines(lines[:idx+8])
EOF
python3 fix_program.py
rm fix_program.py

# Fix DateTime.Now in Notification.cs
sed -i '' 's/DateTime.Now/DateTime.UtcNow/g' NotificationApp.Models/Notification.cs

# Remove unused using in NotificationService.cs
sed -i '' 's/using NotificationSystem.NotificationSenders;//' NotificationApp.Business/NotificationService.cs
sed -i '' '/using NotificationApp.Business.NotificationSenders;/d' NotificationApp.Business/NotificationService.cs
sed -i '' 's/using NotificationApp.Models;/using NotificationApp.Models;\nusing NotificationApp.Business.NotificationSenders;/' NotificationApp.Business/NotificationService.cs

echo "Done"
