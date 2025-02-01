# microservice-architecture
В 8 дз реализована распределенная транзакция с помощью паттерна саги, где управление процессом выполняет оркестратор.

Добавлены три сервиса: авторизации, каталога и расписания. В авторизации появились роли клиента и владельца услуг. В каталоге владелец может добавлять услуги и медицинских работников. В расписании он управляет графиком, а клиент бронирует свободные слоты.

Когда клиент делает бронирование, сначала списываются деньги. Если все прошло успешно, сервис бронирует время в расписании, затем сохраняет запись о бронировании в базе и отправляет уведомление пользователю. Если на каком-то этапе что-то идет не так, выполняются компенсирующие действия. Например, если не удалось забронировать время, деньги возвращаются, а бронь отменяется.

Отправка уведомления не требует компенсации, так как оно не влияет на сам процесс бронирования. Даже если уведомление не дойдет, бронь останется в силе. В результате система работает так, что либо бронирование проходит полностью, либо все изменения отменяются.

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

## Установка RabbitMQ
```bash
helm install rabbitmq bitnami/rabbitmq -f  ./infra/rabbitmq/values.yaml
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
newman run homework8.postman_collection.json
```