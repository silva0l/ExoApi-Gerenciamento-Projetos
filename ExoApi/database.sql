IF DB_ID(N'ExoApi') IS NULL
    CREATE DATABASE ExoApi;
GO

USE ExoApi;
GO

IF OBJECT_ID(N'dbo.Projetos', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Projetos
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Projetos PRIMARY KEY,
        Nome NVARCHAR(120) NOT NULL,
        Descricao NVARCHAR(1000) NOT NULL CONSTRAINT DF_Projetos_Descricao DEFAULT N'',
        Status NVARCHAR(30) NOT NULL CONSTRAINT DF_Projetos_Status DEFAULT N'Planejado',
        CriadoEmUtc DATETIME2 NOT NULL CONSTRAINT DF_Projetos_CriadoEmUtc DEFAULT SYSUTCDATETIME(),
        AtualizadoEmUtc DATETIME2 NOT NULL CONSTRAINT DF_Projetos_AtualizadoEmUtc DEFAULT SYSUTCDATETIME(),
        CONSTRAINT UQ_Projetos_Nome UNIQUE (Nome)
    );
END;
GO
