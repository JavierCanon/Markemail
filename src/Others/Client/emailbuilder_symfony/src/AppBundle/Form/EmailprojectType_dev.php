<?php

namespace AppBundle\Form;

use Symfony\Component\Form\AbstractType;
use Symfony\Component\Form\FormBuilderInterface;
use Symfony\Component\OptionsResolver\OptionsResolver;
use Symfony\Component\Form\Extension\Core\Type\TextType;
use Symfony\Component\Form\Extension\Core\Type\IntegerType;

class EmailprojectType extends AbstractType
{
    /**
     * {@inheritdoc}
     */
    public function buildForm(FormBuilderInterface $builder, array $options)
    {
        $builder->add('exacttarget_id', IntegerType::class,array('required'=>false))->add('title')->add('source', TextType::class,array('required'=>false))->add('campaign', TextType::class,array('required'=>false))->add('medium', TextType::class,array('required'=>false))->add('term', TextType::class,array('required'=>false))->add('content', TextType::class,array('required'=>false));
      /*
              $builder->add('username')->add('body')->add('email')->add('exacttarget_id', IntegerType::class,array('required'=>false))->add('title')->add('company')->add('source', TextType::class,array('required'=>false))->add('campaign', TextType::class,array('required'=>false))->add('medium', TextType::class,array('required'=>false))->add('term', TextType::class,array('required'=>false))->add('content', TextType::class,array('required'=>false));
      */
    }
    
    /**
     * {@inheritdoc}
     */
    public function configureOptions(OptionsResolver $resolver)
    {
        $resolver->setDefaults(array(
            'data_class' => 'AppBundle\Entity\Emailproject'
        ));
    }

    /**
     * {@inheritdoc}
     */
    public function getBlockPrefix()
    {
        return 'appbundle_emailproject';
    }


}



