# 🔍 Análise do Banco de Dados Real vs API

## ❌ Incompatibilidades Identificadas

### 1. **Tabela Aluno - Diferenças Críticas**

**Banco Real:**
```sql
CREATE TABLE Aluno (
    id_aluno INT PRIMARY KEY AUTO_INCREMENT,
    telefone VARCHAR(20),
    cpf VARCHAR(14) UNIQUE NOT NULL,
    endereco VARCHAR(200),
    cidade VARCHAR(100),
    curso VARCHAR(100),
    turno ENUM('Matutino', 'Vespertino', 'Noturno', 'Integral'),
    fk_id_usuario INT NOT NULL,
    fk_id_instituicao INT NOT NULL
);
```

**API Atual (INCORRETA):**
```csharp
[Column("turno")]
public TurnoEnum? _turno { get; set; }  // Enum com valores errados

[Column("status")]
public StatusAlunoEnum _status { get; set; }  // COLUNA NÃO EXISTE!

[Column("data_matricula")]
public DateTime? _dataMatricula { get; set; }  // COLUNA NÃO EXISTE!
```

### 2. **Enums Incorretos**

| Campo | Banco Real | API Atual | Status |
|-------|------------|-----------|---------|
| **turno** | `'Matutino', 'Vespertino', 'Noturno', 'Integral'` | `Manha, Tarde, Noite, Integral` | ❌ **INCOMPATÍVEL** |
| **status** | ❌ **NÃO EXISTE** | `Ativo, Inativo, Suspenso` | ❌ **ERRO** |

### 3. **Outras Tabelas - Enums Corretos**

**Rota:**
```sql
status ENUM('Planejada', 'Em andamento', 'Concluída', 'Cancelada')
tipo_rota ENUM('Ida', 'Volta', 'Circular')
```

**Pagamento:**
```sql
status ENUM('Pago', 'Pendente', 'Atrasado')
forma_pagamento ENUM('PIX', 'Cartão', 'Dinheiro', 'Transferência')
```

**Onibus:**
```sql
status ENUM('Ativo', 'Manutenção', 'Inativo')
```

## 🔧 Correções Necessárias

### **URGENTE - Modelo Aluno**
1. ✅ **Remover campos inexistentes**: `_status`, `_dataMatricula`
2. ✅ **Corrigir enum turno**: Voltar para `Matutino, Vespertino, Noturno`
3. ✅ **Ajustar StudentsController**: Remover referências aos campos inexistentes

### **Estrutura Real do Banco**
- ✅ **Aluno**: Sem status, sem data_matricula
- ✅ **Usuario**: Tem campo `ativo` (boolean)
- ✅ **Relacionamentos**: Corretos via foreign keys

## 📋 Plano de Correção

### **Fase 1: Reverter Modelo Aluno**
```csharp
public enum TurnoEnum
{
    Matutino,    // Volta ao original
    Vespertino,  // Volta ao original
    Noturno,     // Volta ao original
    Integral
}

// REMOVER:
// - StatusAlunoEnum
// - _status
// - _dataMatricula
```

### **Fase 2: Corrigir StudentsController**
- Status do aluno = Status do Usuario (ativo/inativo)
- Data matrícula = Não existe, usar data atual
- Filtros baseados em campos reais

### **Fase 3: Reverter Migration**
- Remover colunas criadas incorretamente
- Voltar ao schema original

## 🎯 Resultado Esperado
- ✅ API alinhada com banco real
- ✅ Enums corretos
- ✅ Campos existentes
- ✅ Frontend funcionando
