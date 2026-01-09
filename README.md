# Payment Gateway Integration

## 📌 Breve Descrição
Módulo robusto desenvolvido em C# para integração com gateways de pagamento, suportando múltiplos métodos e garantindo resiliência e confiabilidade nas transações financeiras.

## 🎯 Problema / Desafio
Integrar-se de forma segura e estável a APIs de pagamento externas, lidando com falhas de comunicação, diferentes métodos de pagamento e a necessidade de rastreabilidade das operações.

## 🧠 Solução
Desenvolvimento de um módulo que abstrai a complexidade das APIs de pagamento, centralizando o processamento das transações e implementando mecanismos de **retry automático**, **timeout controlado** e **logging detalhado** para auditoria.

## 🛠️ Tecnologias Utilizadas
- C#
- .NET Core
- REST APIs
- Tratamento de Exceções
- Logging estruturado

## 👨‍💻 Minhas Contribuições
- Implementação da lógica de processamento multimétodo (Cartão, Boleto e PIX).
- Desenvolvimento do mecanismo de retry automático, com até 3 tentativas em caso de falha.
- Criação de um sistema de logging detalhado para auditoria e rastreamento das transações.
- Implementação de controle de timeout de 30 segundos para requisições externas.

## ⭐ Funcionalidades Chave
- Processamento de pagamentos multimétodo
- Resiliência com retry automático
- Auditoria e logging detalhado
- Controle de conexão via timeout

## ✅ Resultados
Aumentou a confiabilidade das transações financeiras, minimizando perdas causadas por falhas de comunicação e proporcionando rastreabilidade completa das operações.
