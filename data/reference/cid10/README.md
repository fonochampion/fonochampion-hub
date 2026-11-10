# Referência local CID-10

Coloque neste diretório os quatro arquivos CSV CID-10 fornecidos para cada ambiente:

- `CID-10-CAPITULOS.CSV`
- `CID-10-GRUPOS.CSV`
- `CID-10-CATEGORIAS.CSV`
- `CID-10-SUBCATEGORIAS.CSV`

Os arquivos devem manter a codificação e o conteúdo originais. Arquivos CID-O não são usados.
Os dados são de uso interno e não podem ser commitados, empacotados ou redistribuídos. O
`.gitignore` exclui os arquivos deste diretório, mantendo este README rastreável.

Com o PostgreSQL local em execução, importe explicitamente com:

```sh
docker compose run --rm app --import-cid10 /data/cid10
```

A aplicação não importa o catálogo automaticamente ao iniciar. O conjunto importado é
identificado por uma impressão digital de seu conteúdo; ela não representa uma edição oficial.
