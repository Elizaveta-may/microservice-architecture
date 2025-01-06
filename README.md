# microservice-architecture

## PostgreSQL
```bash
helm repo add bitnami https://charts.bitnami.com/bitnami
helm repo update
helm install postgres bitnami/postgresql -f ./infra/postgres/values.yaml
```

## Auth Service
```bash
kubectl create namespace auth-service
helm upgrade --install auth-service ./infra/auth-service/ --namespace auth-service
```

## Deploy Access Management Service
```bash
kubectl create namespace accessmanagement-service
helm upgrade --install accessmanagement-service ./infra/accessmanagement-service --namespace accessmanagement-service
```

## NGINX Ingress Controller
```bash
helm repo add ingress-nginx https://kubernetes.github.io/ingress-nginx
helm repo update
helm install ingress-nginx ingress-nginx/ingress-nginx
kubectl apply -f infra/nginx-ingress/ingress.yaml
```
