apiVersion: apps/v1
kind: Deployment
metadata:
  name: mailpit-smtp-{{ENV}}
spec:
  replicas: 1
  selector:
    matchLabels:
      app: mailpit-smtp-{{ENV}}
  template:
    metadata:
      labels:
        app: mailpit-smtp-{{ENV}}
    spec:
      containers:
        - name: mailpit
          image: axllent/mailpit:latest
          args: ["--database=https://{{RQLITE_URL}}"]
          ports:
            - containerPort: 1025
