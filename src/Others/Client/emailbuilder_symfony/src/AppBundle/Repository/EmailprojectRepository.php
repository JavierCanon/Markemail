<?php 
// src/AppBundle/Repository/UserRepository.php
namespace AppBundle\Repository;

use Doctrine\ORM\EntityRepository;

class EmailprojectRepository extends EntityRepository
{
    public function loadUserProjects2($username)
    { //this works, but I couldn't get it to work with $username variable
        return $this->getEntityManager()
            ->createQuery(
              'SELECT p FROM AppBundle:Emailproject p WHERE p.username = \'pete\' '
            )
            ->getResult();
    }
    public function loadUserProjects($username)
    {
        return $this->getEntityManager()
            ->createQueryBuilder('p')
            ->select('p')
            ->from('AppBundle:Emailproject','p')
            ->where('p.username = :username')
            ->setParameter('username',$username)
            ->getQuery()
            ->getResult();
    }

}
// DO STUFF!