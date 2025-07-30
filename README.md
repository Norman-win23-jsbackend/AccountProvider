# 🔐 AccountProvider

**AccountProvider** is a secure, serverless backend API developed with **.NET 8** and **Azure Functions**.  
It provides account management functionality — such as authentication, user identity, and secure data access — using modern Azure-based architecture.

---

## 🚀 Features

- ⚙️ Built with **Azure Functions** for scalable, serverless execution  
- 🔐 Uses **Azure.Identity** for secure connections to Azure resources  
- 📦 Modular architecture with a separate Data layer (Data.dll)  
- 🧩 Ready for gRPC and Protobuf-based communication  
- 🧱 Lightweight and cloud-ready project structure  

---

## 📁 Project Structure

```

AccountProvider/
├── Program.cs               # Main entry point
├── host.json               # Azure Functions host configuration
├── local.settings.json     # Local development settings (excluded from Git)
├── AccountProvider.csproj  # Project file
├── Data/                   # Data layer (compiled into Data.dll)
└── Functions/              # Azure Function endpoints

````

---

## 🛠️ Getting Started

### 1. Install Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [Azure Functions Core Tools](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local)

### 2. Clone and Run the Project

```bash
git clone https://github.com/Norman-Deen/AccountProvider.git
cd AccountProvider
func start
````

Navigate to:

```
http://localhost:7071
```

---

## ☁️ Azure Integration

* Uses `Azure.Identity` for secure service-to-service authentication
* Works with Azure Key Vault, Cosmos DB, and API Management
* Easily extendable with Azure Active Directory or B2C

---

## 🔐 Security Best Practices

* Do not commit `local.settings.json` — it contains sensitive data
* Store secrets in environment variables or Azure Key Vault
* Configure proper CORS and authentication for production

---

## 📄 License

This project is open for learning and demonstration purposes.
Licensed under the [MIT License](LICENSE).

---

 **Nour Tinawi**
[LinkedIn](https://www.linkedin.com/in/nour-tinawi) • [Portfolio](https://www.pure-art.co) • [GitHub](https://github.com/Norman-Deen)
