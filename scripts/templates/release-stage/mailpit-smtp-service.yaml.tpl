apiVersion: v1
kind: Service
metadata:
  name: mailpit-smtp-{{ENV}}
  annotations:
    cloud.google.com/load-balancer-type: "Internal"
spec:
  selector:
    app: mailpit-smtp-{{ENV}}
  ports:
    - port: 1025
      targetPort: 1025
  type: LoadBalancer
