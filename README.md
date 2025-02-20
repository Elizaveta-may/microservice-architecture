# microservice-architecture

## Чарты сервисов
```bash
infra/
```

## Подготовка docker-образов
```bash
cd src
docker build -t elizavetamay/medvisit_accessmanagement:projectwork -f accessmanagement.Dockerfile .
docker build -t elizavetamay/medvisit_authserver:projectwork -f authserver.Dockerfile .
docker build -t elizavetamay/medvisit_bookingservice:projectwork -f bookingservice.Dockerfile .
docker build -t elizavetamay/medvisit_catalogservice:projectwork -f catalogservice.Dockerfile .
docker build -t elizavetamay/medvisit_scheduleservice:projectwork -f scheduleservice.Dockerfile .
docker build -t elizavetamay/medvisit_paymentservice:projectwork -f paymentservice.Dockerfile .
docker build -t elizavetamay/medvisit_notificationservice:projectwork -f notificationservice.Dockerfile .

docker push elizavetamay/medvisit_accessmanagement:projectwork
docker push elizavetamay/medvisit_authserver:projectwork
docker push elizavetamay/medvisit_catalogservice:projectwork
docker push elizavetamay/medvisit_scheduleservice:projectwork
docker push elizavetamay/medvisit_bookingservice:projectwork
docker push elizavetamay/medvisit_paymentservice:projectwork
docker push elizavetamay/medvisit_notificationservice:projectwork
```

## Установка Prometheus Grafana
```bash
helm upgrade --install prometheus prometheus-community/kube-prometheus-stack --namespace monitoring --create-namespace -f infra/prometheus/prometheus.yaml
kubectl port-forward svc/prometheus-grafana  3000:80 -n monitoring
kubectl get secret prometheus-grafana -n monitoring -o jsonpath="{.data.admin-password}" | ForEach-Object { [System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String($_)) }
```

## Установка RabbitMQ
```bash
helm install rabbitmq bitnami/rabbitmq -f  ./infra/rabbitmq/values.yaml
```

## Установка Redis
```bash
helm install redis bitnami/redis -f ./infra/redis/values.yaml
```

## Установка Auth PostgreSQL
```bash
helm repo add bitnami https://charts.bitnami.com/bitnami
helm repo update
helm install auth-postgres bitnami/postgresql -f ./infra/postgres/auth.values.yaml
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
  
## Установка Payment Service и бд
```bash
kubectl create namespace payment-service
helm install payment-postgres bitnami/postgresql -f ./infra/postgres/payment.values.yaml  --namespace payment-service 
helm upgrade --install payment-service ./infra/payment-service/ --namespace payment-service   
```
 
## Установка Booking Service и бд
```bash
kubectl create namespace booking-service
helm install booking-postgres bitnami/postgresql -f ./infra/postgres/booking.values.yaml --namespace booking-service 
helm upgrade --install booking-service ./infra/booking-service/ --namespace booking-service   
```

## Установка Catalog Service и бд
```bash
kubectl create namespace catalog-service
helm install catalog-postgres bitnami/postgresql -f ./infra/postgres/catalog.values.yaml --namespace catalog-service 
helm upgrade --install catalog-service ./infra/catalog-service/ --namespace catalog-service   
```

## Установка Schegule Service и бд
```bash
kubectl create namespace schedule-service
helm install schedule-postgres bitnami/postgresql -f ./infra/postgres/schedule.values.yaml --namespace schedule-service 
helm upgrade --install schedule-service ./infra/schedule-service/ --namespace schedule-service   
```

## Установка Notification Service и бд
```bash
kubectl create namespace notification-service
helm install notification-postgres bitnami/postgresql -f ./infra/postgres/notification.values.yaml --namespace notification-service 
helm upgrade --install notification-service ./infra/notification-service/ --namespace notification-service  
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
helm uninstall auth-postgres
helm uninstall ingress-nginx
helm uninstall rabbitmq
helm uninstall redis
helm uninstall prometheus -n monitoring
helm uninstall accessmanagement-service -n accessmanagement-service
helm uninstall auth-service -n auth-service 

helm uninstall payment-postgres -n payment-service
helm uninstall payment-service -n payment-service

helm uninstall booking-postgres -n booking-service
helm uninstall booking-service -n booking-service

helm uninstall catalog-postgres -n catalog-service
helm uninstall catalog-service -n catalog-service

helm uninstall schedule-postgres -n schedule-service
helm uninstall schedule-service -n schedule-service

helm uninstall notification-postgres -n notification-service
helm uninstall notification-service -n notification-service
```

## Тестирование
```bash
newman run Homework9.postman_collection.json
```