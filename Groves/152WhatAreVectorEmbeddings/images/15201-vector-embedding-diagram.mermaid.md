graph LR
    A[Object 1] --> E(Embedding Model)
    C[Object 2] --> E
    D[Object 3] --> E
    E{Embedding Model}
    
    E -->|Vector 1| F([#91;0.12, 0.75, -0.33 ... #93;])
    E -->|Vector 2| G([#91;0.85, 0.21, 0.88 ... #93;])
    E -->|Vector 3| H([#91;-0.05, 0.68, 0.5 ... #93;])
