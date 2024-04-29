graph LR
    A[Images] -->|Embedding| B(Dense vectors, 
    stored in a database,
    alongside original content)
    C[Documents] -->|Embedding| B
    D[Audio] -->|Embedding| B
    B --> E[Search Database for
     nearest neighbors]
    F[Query] -->|Embedding| G(Vector representation)
    G --> E
    E --> H[Semantically Similar
     Results]
