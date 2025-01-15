# microservice-architecture

## Чарты сервисов
```bash
infra/
```

## Подготовка docker-образов
```bash
cd src
docker build -t elizavetamay/medvisit_authserver:auth1 -f authserver.Dockerfile .
docker push elizavetamay/medvisit_authserver:auth1
docker build -t elizavetamay/medvisit_accessmanagement:app_1 -f accessmanagement.Dockerfile .
docker push elizavetamay/medvisit_accessmanagement:app_1
```

## Установка PostgreSQL
```bash
helm repo add bitnami https://charts.bitnami.com/bitnami
helm repo update
helm install postgres bitnami/postgresql -f ./infra/postgres/values.yaml
```

## Установка Auth Service
```bash
kubectl create namespace auth-service
helm upgrade --install auth-service ./infra/auth-service/ --namespace auth-service
```

## Установка Access Management Service
```bash
kubectl create namespace accessmanagement-service
helm upgrade --install accessmanagement-service ./infra/accessmanagement-service --namespace accessmanagement-service
```

## Установка NGINX Ingress Controller (API GATEWAY)
```bash
helm install ingress-nginx ingress-nginx/ingress-nginx
kubectl apply -f infra/nginx-ingress/ingress.yaml
```
   
## Перенаправление порта для доступа к Ingress NGINX
```bash
kubectl port-forward svc/ingress-nginx-controller 80:80
``` 

## Удаление
```bash
helm uninstall postgres
helm uninstall ingress-nginx
helm uninstall accessmanagement-service -n accessmanagement-service
helm uninstall auth-service -n auth-service 
```

## Тестирование
```bash
newman run homework6.postman_collection.json
```